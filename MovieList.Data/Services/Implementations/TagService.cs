using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;
using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

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
