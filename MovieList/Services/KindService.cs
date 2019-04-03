using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MovieList.Data;
using MovieList.Data.Models;

namespace MovieList.Services
{
    public class KindService : IKindService
    {
        private readonly MovieContext context;

        public KindService(MovieContext context)
            => this.context = context;

        public async Task<ObservableCollection<Kind>> LoadAllKindsAsync()
            => new ObservableCollection<Kind>(await this.context.Kinds.OrderBy(k => k.Name).ToListAsync());

        public async Task SaveKindsAsync(IEnumerable<Kind> kinds)
        {
            var dbKinds = await this.context.Kinds.AsNoTracking().ToListAsync();

            foreach (var kind in kinds)
            {
                if (await this.context.Kinds.ContainsAsync(kind))
                {
                    this.context.Attach(kind).State = EntityState.Modified;
                } else
                {
                    this.context.Kinds.Add(kind);
                }
            }

            foreach (var kind in dbKinds.Except(kinds, IdEqualityComparer<Kind>.Instance))
            {
                this.context.Attach(kind).State = EntityState.Deleted;
            }

            await this.context.SaveChangesAsync();
        }
    }
}
