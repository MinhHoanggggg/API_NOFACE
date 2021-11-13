using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace API_Noface.Models
{
    public partial class NofaceDbContext : DbContext
    {
        public NofaceDbContext()
            : base("name=NofaceDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Avt> Avt { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Friends> Friends { get; set; }
        public virtual DbSet<Likes> Likes { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Topic> Topic { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Avt>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Comment>()
                .Property(e => e.IDUser)
                .IsFixedLength();

            modelBuilder.Entity<Friends>()
                .Property(e => e.IDUser)
                .IsFixedLength();

            modelBuilder.Entity<Friends>()
                .Property(e => e.IDFriends)
                .IsFixedLength();

            modelBuilder.Entity<Likes>()
                .Property(e => e.IDUser)
                .IsFixedLength();

            modelBuilder.Entity<Post>()
                .Property(e => e.IDUser)
                .IsFixedLength();

            modelBuilder.Entity<Post>()
                .Property(e => e.ImagePost)
                .IsUnicode(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Post)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Likes)
                .WithRequired(e => e.Post)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Topic>()
                .Property(e => e.Img)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .Property(e => e.IDUser)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Friends)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IDFriends);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Friends1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.IDUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Likes)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
