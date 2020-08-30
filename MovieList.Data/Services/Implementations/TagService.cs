using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal class TagService : ServiceBase, ITagService
    {
        public TagService(string file)
            : base(file)
        { }

        public IEnumerable<Tag> GetAllTags()
        {
            this.Log().Debug("Getting all tags");
            return this.WithTransaction(this.GetAllTags);
        }

        private IEnumerable<Tag> GetAllTags(IDbConnection connection, IDbTransaction transaction)
        {
            var tags = connection.GetAll<Tag>(transaction);
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
    }
}
