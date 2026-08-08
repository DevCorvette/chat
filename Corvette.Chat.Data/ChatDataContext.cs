using Corvette.Chat.Data.Entities;
using Curiosity.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace Corvette.Chat.Data
{
    public class ChatDataContext : CuriosityDataContext<ChatDataContext>
    {
        public ChatDataContext() : base(new DbContextOptions<ChatDataContext>())
        {
        }

        public ChatDataContext(DbContextOptions<ChatDataContext> options) : base(options)
        {
        }
        
        // it using for EF migrator
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("some connection string");
            }
        }

        public virtual DbSet<UserEntity> Users { get; set; } = null!;

        public virtual DbSet<ChatEntity> Chats { get; set; } = null!;

        public virtual DbSet<MessageEntity> Messages { get; set; } = null!;

        public virtual DbSet<MemberEntity> Members { get; set; } = null!;


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

                entity.Property(e => e.LastReadDate)
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