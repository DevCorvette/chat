using System;
using System.Threading.Tasks;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.Services.Extensions
{
    /// <summary>
    /// Extensions for DbSet.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Returns entity by id.
        /// </summary>
        /// <param name="dbSet">Db set</param>
        /// <param name="id"></param>
        /// <param name="asNoTracking">Returns entity as no tracking when it's true</param>
        /// <typeparam name="T">Entity</typeparam>
        /// <exception cref="ArgumentOutOfRangeException">When id is default</exception>
        /// <exception cref="EntityNotFoundException">When an entity was not found by id</exception>
        public static async Task<T> GetByIdAsync<T>(this DbSet<T> dbSet, Guid id, bool asNoTracking = false) 
            where T : BaseEntity
        {
            if (id == default) throw new ArgumentOutOfRangeException(nameof(id), "Can't be default");

            
            var query = asNoTracking ? dbSet.AsNoTracking() : dbSet.AsQueryable();
            
            var entity = await query.SingleOrDefaultAsync(x => x.Id == id);

            return entity ?? throw new EntityNotFoundException($"Entity was not found by id: {id}.");
        }
    }
}