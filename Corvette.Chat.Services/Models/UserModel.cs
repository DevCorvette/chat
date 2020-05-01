using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Services.Models
{
    /// <summary>
    /// Chat user DTO.
    /// </summary>
    public sealed class UserModel
    {
        /// <summary>
        /// User id.
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// Date when user was created.
        /// </summary>
        public DateTime Created { get; }
        
        /// <summary>
        /// User name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new <see cref="UserModel"/>
        /// </summary>
        public UserModel(UserEntity entity)
        {
            Id = entity.Id;
            Created = entity.Created;
            Name = entity.Name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, " +
                   $"{nameof(Created)}: {Created}, " +
                   $"{nameof(Name)}: {Name}";
        }
    }
}