using System;
using System.ComponentModel.DataAnnotations;

namespace Corvette.Chat.Data.Entities
{
    /// <summary>
    /// Base class for all entities.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Unique id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Date when entity was created.
        /// It's auto-generated in the database when an entity is inserted.
        /// </summary>
        public DateTime Created { get; set; }
    }
}