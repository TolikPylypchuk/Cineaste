using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MovieList.Data;
using MovieList.Data.Models;
using MovieList.ViewModels;

namespace MovieList.Services.Implementations
{
    public class KindService : IKindService
    {
        private readonly MovieContext context;

        public KindService(MovieContext context)
            => this.context = context;

        public async Task<ObservableCollection<KindViewModel>> LoadAllKindsAsync()
            => new ObservableCollection<KindViewModel>(
                await this.context.Kinds
                    .OrderBy(k => k.Name)
                    .Select(k => new KindViewModel(k))
                    .ToListAsync());

        public async Task SaveKindsAsync(IEnumerable<KindViewModel> kinds)
        {
            if (kinds.Any(k => k.HasErrors))
            {
                throw new ArgumentException("Cannot save invalid kinds.", nameof(kinds));
            }

            var dbKinds = await this.context.Kinds.AsNoTracking().ToListAsync();
            var kindsToSave = kinds.Select(k => k.Kind).ToList();

            foreach (var kind in kindsToSave)
            {
                if (await this.context.Kinds.ContainsAsync(kind))
                {
                    this.context.Attach(kind).State = EntityState.Modified;
                } else
                {
                    this.context.Kinds.Add(kind);
                }
            }

            foreach (var kind in dbKinds.Except(kindsToSave, IdEqualityComparer<Kind>.Instance))
            {
                this.context.Attach(kind).State = EntityState.Deleted;
            }

            await this.context.SaveChangesAsync();
        }
    }
}
