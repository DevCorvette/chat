using Corvette.Chat.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.Data
{
    public class ChatDataContext : DbContext
    {
        private readonly string? _connectionString;

        private readonly bool _isTest;

        public ChatDataContext()
        {
        }

        public ChatDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public ChatDataContext(DbContextOptions<ChatDataContext> options, bool isTest)
            : base(options)
        {
            _isTest = isTest;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!_isTest)
            {
                optionsBuilder.UseNpgsql(_connectionString ?? "Connection string is null"); // I use default string for EF migration
            }
        }


        public virtual DbSet<UserEntity> Users { get; set; } = null!;
        
        public virtual DbSet<ChatEntity> Chats { get; set; } = null!;
        
        public virtual DbSet<MessageEntity> Messages { get; set; } = null!;
        
        public virtual DbSet<MemberEntity> ChatUsers { get; set; } = null!;

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("timezone('UTC'::text, now())"); // postgres function
                
                entity.HasIndex(x => x.Name)
                    .IsUnique();

                entity.HasIndex(x => x.Login)
                    .IsUnique();

                entity.HasIndex(x => new {x.Login, x.SecretKey}); // it's used for authorization
            });

            builder.Entity<MessageEntity>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("timezone('UTC'::text, now())"); // postgres function
                
                entity.HasOne(x => x.Author)
                    .WithMany(x => x!.Messages)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Chat)
                    .WithMany(x => x!.Messages)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            builder.Entity<MemberEntity>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("timezone('UTC'::text, now())"); // postgres function
                
                entity.HasOne(x => x.User)
                    .WithMany(x => x!.ChatUsers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Chat)
                    .WithMany(x => x!.ChatUsers)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new {x.ChatId, x.UserId})
                    .IsUnique();
            });

            builder.Entity<ChatEntity>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("timezone('UTC'::text, now())"); // postgres function
                
                entity.HasOne(x => x.Owner)
                    .WithMany(x => x!.OwnChats)
                    .HasForeignKey(x => x.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}