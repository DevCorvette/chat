using Corvette.Chat.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.Data
{
    public class ChatDataContext : DbContext
    {
        private readonly string? _connectionString;

        public ChatDataContext()
        {
        }

        public ChatDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString ?? "Connection string is null"); // I use default string for EF migration
        }


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