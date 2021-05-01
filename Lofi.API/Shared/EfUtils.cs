using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                Expression<Func<TEntity, IEnumerable<TCollectionEntity>>> collectionSelector,
                Expression<Func<TCollectionEntity, TCollectionEntityKey>> keySelector,
                IReadOnlyCollection<TCollectionEntityKey> keys,
                CancellationToken cancellationToken = default
        )
            where TDbContext : DbContext
            where TEntity : class
            where TCollectionEntity : class
        {
            /* you can't load a collection of an entity which has been detached from
             * the ef graph. Typically this will be happening because the entity is
             * just a new object.
             */
            if (dbContext.Entry(entity).State != EntityState.Detached)
            {
                await dbContext.Entry(entity)
                    .Collection<TCollectionEntity>(collectionSelector)
                    .LoadAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
           
            var compiledCollectionSelector = collectionSelector.Compile();
            var compiledKeySelector = keySelector.Compile();
            
            var collection = compiledCollectionSelector.Invoke(entity) as ICollection<TCollectionEntity>;
            if (collection == null) throw new NotImplementedException("Support for entity collections which are not ICollection<T> has not been added.");
            var currentKeys = collection.Select(compiledKeySelector).ToHashSet();
            var keysToRemove = currentKeys.Except(keys);
            var keysToAdd = keys.Except(currentKeys);

            
            var itemsToRemove = collection
                .Where(item => keysToRemove.Contains(compiledKeySelector(item)));
            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }

            var parameter = Expression.Parameter(typeof(TCollectionEntity), "entity");
            var filter = Expression.Lambda(
                Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Contains),
                    new [] { typeof(TCollectionEntityKey) },
                    Expression.Constant(keysToAdd),
                    Expression.Invoke(
                        keySelector,
                        parameter
                    )
                ),
                parameter
            );
            var itemsToAdd = await dbContext.Set<TCollectionEntity>()
                .Where((Expression<Func<TCollectionEntity, bool>>)filter)
                .ToListAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var item in itemsToAdd)
            {
                collection.Add(item);
            }
        }
    }
}