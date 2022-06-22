using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VrijenhoekPhotos.Exchange.Classes;
using VrijenhoekPhotos.Persistence.RelationClasses;

namespace VrijenhoekPhotos.Persistence
{
    public class VrijenhoekPhotosDbContext : DbContext
    {
        public class OptionsBuild
        {
            public OptionsBuild()
            {
                settings = new AppConfiguration();
                opsBuilder = new DbContextOptionsBuilder<VrijenhoekPhotosDbContext>();
                opsBuilder.UseSqlServer(settings.sqlConnectionString);
                dbOptions = opsBuilder.Options;
            }
            public DbContextOptionsBuilder<VrijenhoekPhotosDbContext> opsBuilder { get; set; }

            public DbContextOptions<VrijenhoekPhotosDbContext> dbOptions { get; set; }

            private AppConfiguration settings { get; set; }
        }

        public static OptionsBuild ops = new OptionsBuild();

        public VrijenhoekPhotosDbContext(DbContextOptions<VrijenhoekPhotosDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDTO>().HasKey(x => x.Id);
            builder.Entity<UserDTO>().HasMany(x => x.Groups).WithMany(x => x.Users).UsingEntity<GroupUsers>(x => x.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId), x => x.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId)); ;
            builder.Entity<UserDTO>().HasMany(x => x.Albums).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<UserDTO>().HasMany(x => x.PendingJoinGroups).WithMany(x => x.PendingJoinUsers).UsingEntity<GroupInvitedUsers>(x => x.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId), x => x.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId));
            builder.Entity<UserDTO>().HasMany(x => x.RequestedJoinGroups).WithMany(x => x.RequestedJoinUsers).UsingEntity<GroupRequestingUsers>(x => x.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId), x => x.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId));

            builder.Entity<GroupUsers>().HasKey(x => new { x.GroupId, x.UserId });

            builder.Entity<GroupDTO>().HasKey(x => x.Id);
            builder.Entity<GroupDTO>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<GroupDTO>().HasMany(x => x.Albums).WithMany(x => x.Groups).UsingEntity<GroupAlbums>(x => x.HasOne(x => x.Album).WithMany().HasForeignKey(x => x.AlbumId), x => x.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId));
            builder.Entity<GroupDTO>().HasOne(x => x.Creator).WithMany(x => x.OwnedGroups).HasForeignKey(x => x.CreatorId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<GroupDTO>().HasOne(x => x.IconPhoto).WithMany().HasForeignKey(x => x.IconPhotoId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);

            builder.Entity<AlbumDTO>().HasKey(x => x.Id);
            builder.Entity<AlbumDTO>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<AlbumDTO>().HasMany(x => x.Photos).WithMany(x => x.Albums).UsingEntity<AlbumPhotos>(x => x.HasOne(x => x.Photo).WithMany().HasForeignKey(x => x.PhotoId), x => x.HasOne(x => x.Album).WithMany().HasForeignKey(x => x.AlbumId));
            builder.Entity<AlbumDTO>().HasOne(x => x.IconPhoto).WithMany().HasForeignKey(x => x.IconPhotoId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            builder.Entity<AlbumDTO>().HasMany(x => x.Tags).WithMany(x => x.Albums).UsingEntity<AlbumTags>(x => x.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId), x => x.HasOne(x => x.Album).WithMany().HasForeignKey(x => x.AlbumId));

            builder.Entity<PhotoDTO>().HasKey(x => x.Id);
            builder.Entity<PhotoDTO>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<PhotoDTO>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

            builder.Entity<TagDTO>().HasKey(x => x.Id);
            builder.Entity<TagDTO>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<TagDTO>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        }

        public DbSet<T> GetTable<T>(string value) where T : class
        {
            return typeof(VrijenhoekPhotosDbContext).GetProperty(value)?.GetValue(this) as DbSet<T>;
        }

        public DbSet<UserDTO> Users { get; set; }
        public DbSet<GroupUsers> GroupUsers { get; set; }
        public DbSet<GroupInvitedUsers> GroupInvitedUsers { get; set; }
        public DbSet<GroupRequestingUsers> GroupRequestingUsers { get; set; }
        public DbSet<GroupDTO> Groups { get; set; }
        public DbSet<GroupAlbums> GroupAlbums { get; set; }
        public DbSet<AlbumDTO> Albums { get; set; }
        public DbSet<AlbumTags> AlbumTags { get; set; }
        public DbSet<TagDTO> Tags { get; set; }
        public DbSet<AlbumPhotos> AlbumPhotos { get; set; }
        public DbSet<PhotoDTO> Photos { get; set; }
        
    }
}
