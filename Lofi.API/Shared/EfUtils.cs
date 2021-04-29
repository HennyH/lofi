using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Shared
{
    public static class EfUtils
    {
        public static  async Task SynchronizeCollectionByKey<TDbContext, TEntity, TCollectionEntity, TCollectionEntityKey>(
                TDbContext dbContext,
                TEntity entity,
                Func<TEntity, ICollection<TCollectionEntity>> collectionSelector,
                Func<TCollectionEntity, TCollectionEntityKey> keySelector,
                IReadOnlyCollection<TCollectionEntityKey> keys,
                CancellationToken cancellationToken = default
        )
            where TDbContext : DbContext
            where TEntity : class
            where TCollectionEntity : class
        {
            await dbContext.Entry(entity)
                .Collection<TCollectionEntity>(e => collectionSelector(e))
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var collection = collectionSelector(entity);
            var currentKeys = collection.Select(keySelector).ToHashSet();
            var keysToRemove = currentKeys.Except(keys);
            var keysToAdd = keys.Except(currentKeys);

            var itemsToRemove = collection
                .Where(item => keysToRemove.Contains(keySelector(item)));
            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }

            var itemsToAdd = await dbContext.Set<TCollectionEntity>()
                .Where(e => keysToRemove.Contains(keySelector(e)))
                .ToListAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }
        }
    }
}