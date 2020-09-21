using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;
using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal class TagService : SettingsEntityServiceBase<Tag>
    {
        public TagService(string file)
            : base(file)
        { }

        protected override string GetAllMessage
            => "Getting all tags";

        protected override string UpdateAllMessage
            => "Updating all tags";

        protected override string DeleteExceptionMessage
            => "Cannot delete tags that have movies, series or franchises attached to them";

        protected override bool CanDelete(Tag tag)
            => true;

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

        protected override void AfterSave(Tag tag, IDbConnection connection, IDbTransaction transaction)
        {
            var implications = tag.ImpliedTags
                .Select(impliedTag => new TagImplication { PremiseId = tag.Id, ConsequenceId = impliedTag.Id })
                .Union(tag.InferredTags
                    .Select(inferredTag => new TagImplication { PremiseId = inferredTag.Id, ConsequenceId = tag.Id }))
                .ToList();

            var dbImplications = connection.Query<TagImplication>(
                $"SELECT * FROM TagImplications WHERE PremiseId = @Id OR ConsequenceId = @Id",
                new { tag.Id },
                transaction);

            foreach (var implicationToInsert in implications.Except(
                dbImplications, CompositeIdEqualityComparer.TagImplication))
            {
                implicationToInsert.Id = (int)connection.Insert(implicationToInsert, transaction);
            }

            var implicationsToDelete = dbImplications
                .Except(implications, CompositeIdEqualityComparer.TagImplication)
                .ToList();

            connection.Delete(implicationsToDelete, transaction);
        }

        protected override void BeforeDelete(Tag tag, IDbConnection connection, IDbTransaction transaction)
        {
            base.BeforeDelete(tag, connection, transaction);

            connection.Execute(
                "DELETE FROM TagImplications WHERE PremiseId = @Id OR ConsequenceId = @Id",
                new { tag.Id },
                transaction);

            connection.Execute("DELETE FROM MovieTags WHERE TagId = @Id", new { tag.Id }, transaction);
            connection.Execute("DELETE FROM SeriesTags WHERE TagId = @Id", new { tag.Id }, transaction);
            connection.Execute("DELETE FROM FranchiseTags WHERE TagId = @Id", new { tag.Id }, transaction);
        }
    }
}
