using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

using MovieList.Data.Models;

using ReactiveUI;

namespace MovieList.Data.Services
{
    public static class ServiceExtensions
    {
        public static IObservable<Unit> CreateDatabaseInTaskPool(
            this IDatabaseService service, Settings settings, IEnumerable<Kind> initialKinds)
            => Observable.Start(() => service.CreateDatabase(settings, initialKinds), RxApp.TaskpoolScheduler);

        public static IObservable<bool> ValidateDatabaseInTaskPool(this IDatabaseService service)
            => Observable.Start(service.ValidateDatabase, RxApp.TaskpoolScheduler);

        public static IObservable<Unit> SaveInTaskPool<TEntity>(this IEntityService<TEntity> service, TEntity entity)
            => Observable.Start(() => service.Save(entity), RxApp.TaskpoolScheduler);

        public static IObservable<Unit> DeleteInTaskPool<TEntity>(this IEntityService<TEntity> service, TEntity entity)
            => Observable.Start(() => service.Delete(entity), RxApp.TaskpoolScheduler);

        public static IObservable<IEnumerable<TEntity>> GetAllInTaskPool<TEntity>(
            this ISettingsEntityService<TEntity> service)
            where TEntity : EntityBase
            => Observable.Start(service.GetAll, RxApp.TaskpoolScheduler);

        public static IObservable<Unit> UpdateAllInTaskPool<TEntity>(
            this ISettingsEntityService<TEntity> service, IEnumerable<TEntity> entities)
            where TEntity : EntityBase
            => Observable.Start(() => service.UpdateAll(entities), RxApp.TaskpoolScheduler);

        public static IObservable<MovieList> GetListInTaskPool(
            this IListService service,
            IEnumerable<Kind> kinds,
            IEnumerable<Tag> tags)
            => Observable.Start(() => service.GetList(kinds, tags), RxApp.TaskpoolScheduler);

        public static IObservable<Settings> GetSettingsInTaskPool(this ISettingsService service)
            => Observable.Start(service.GetSettings, RxApp.TaskpoolScheduler);

        public static IObservable<Unit> UpdateSettingsInTaskPool(this ISettingsService service, Settings settings)
            => Observable.Start(() => service.UpdateSettings(settings), RxApp.TaskpoolScheduler);
    }
}
