using System;
using System.ComponentModel.DataAnnotations;

namespace Corvette.Chat.DAL.Entities
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
        /// </summary>
        public DateTime Created { get; set; }
    }
}