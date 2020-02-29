using Corvette.Chat.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.DAL
{
    public class ChatDataContext : DbContext
    {
        public virtual DbSet<UserEntity> Users { get; set; } = null!;
        
        public virtual DbSet<ChatEntity> Chats { get; set; } = null!;
        
        public virtual DbSet<MessageEntity> Messages { get; set; } = null!;
        
        public virtual DbSet<ChatUserEntity> ChatUsers { get; set; } = null!;

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>(entity =>
            {
                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            builder.Entity<MessageEntity>(entity =>
            {
                entity.HasOne(x => x.Author)
                    .WithMany(x => x!.Messages)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Chat)
                    .WithMany(x => x!.Messages)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            builder.Entity<ChatUserEntity>(entity =>
            {
                entity.HasOne(x => x.User)
                    .WithMany(x => x!.ChatUsers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Chat)
                    .WithMany(x => x!.ChatUsers)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}