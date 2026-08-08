using System;
using Corvette.Chat.Data.Entities;

namespace Corvette.Chat.Logic.Models
{
    /// <summary>
    /// Chat user DTO.
    /// </summary>
    public sealed class UserModel
    {
        /// <summary>
        /// User id.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Date when user was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string Name { get; set; } = null!;

        public UserModel()
        {
        }

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