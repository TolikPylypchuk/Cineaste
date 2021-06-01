using System.Collections.Generic;
using System.Data;
using System.Linq;

using Cineaste.Data.Models;

using Dapper;
using Dapper.Contrib.Extensions;

namespace Cineaste.Data.Services.Implementations
{
    internal class TagService : SettingsEntityServiceBase<Tag>
    {
        public TagService(string file)
            : base(file)
        { }

        protected override string GetAllMessage => "Getting all tags";

        protected override string UpdateAllMessage => "Updating all tags";

        protected override string DeleteExceptionMessage =>
            "Cannot delete tags that have movies or series attached to them";

        protected override bool CanDelete(Tag tag) =>
            true;

        protected override IEnumerable<Tag> GetAll(IDbConnection connection, IDbTransaction transaction)
        {
            var tags = base.GetAll(connection, transaction);
            var tagImplications = connection.GetAll<TagImplication>(transaction);

            var tagsById = tags.ToDictionary(tag => tag.Id, tag => tag);

            foreach (var implication in tagImplications)
            {
                var premise = tagsById[implication.PremiseId];
                var consequence = tagsById[implication.ConsequenceId];

                premise.ImpliedTags.Add(consequence);
                consequence.InferredTags.Add(premise);
            }

            return tags;
        }

        protected override void AfterSave(
            Tag tag,
            List<Tag> allTags,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            var implications = tag.ImpliedTags
                .Select(impliedTag => new TagImplication { PremiseId = tag.Id, ConsequenceId = impliedTag.Id })
                .ToList();

            var allTagsById = allTags.ToDictionary(t => t.Id, t => t);

            var dbImplications = connection.Query<TagImplication>(
                $"SELECT * FROM TagImplications WHERE PremiseId = @Id", new { tag.Id }, transaction);

            foreach (var implicationToInsert in implications.Except(
                dbImplications, CompositeIdEqualityComparer.TagImplication))
            {
                implicationToInsert.Id = (int)connection.Insert(implicationToInsert, transaction);
                allTagsById[implicationToInsert.ConsequenceId].InferredTags.Add(tag);
            }

            var implicationsToDelete = dbImplications
                .Except(implications, CompositeIdEqualityComparer.TagImplication)
                .ToList();

            foreach (var implicationToDelete in implicationsToDelete)
            {
                allTagsById[implicationToDelete.ConsequenceId].InferredTags.Remove(tag);
            }

            connection.Delete(implicationsToDelete, transaction);
        }

        protected override void BeforeDelete(
            Tag tag,
            List<Tag> allTags,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            base.BeforeDelete(tag, allTags, connection, transaction);

            foreach (var otherTag in allTags)
            {
                otherTag.ImpliedTags.Remove(tag);
                otherTag.InferredTags.Remove(tag);
            }

            foreach (var movie in tag.Movies)
            {
                movie.Tags.Remove(tag);
            }

            foreach (var series in tag.Series)
            {
                series.Tags.Remove(tag);
            }

            connection.Execute(
                "DELETE FROM TagImplications WHERE PremiseId = @Id OR ConsequenceId = @Id",
                new { tag.Id },
                transaction);

            connection.Execute("DELETE FROM MovieTags WHERE TagId = @Id", new { tag.Id }, transaction);
            connection.Execute("DELETE FROM SeriesTags WHERE TagId = @Id", new { tag.Id }, transaction);
        }
    }
}
