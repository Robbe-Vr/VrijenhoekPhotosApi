﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VrijenhoekPhotos.Persistence;

namespace VrijenhoekPhotos.Persistence.Migrations
{
    [DbContext(typeof(VrijenhoekPhotosDbContext))]
    [Migration("20220301172110_managed-relations")]
    partial class managedrelations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IconPhotoId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("IconPhotoId");

                    b.HasIndex("UserId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.GroupDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IconPhotoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("IconPhotoId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsVideo")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.TagDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.UserDTO", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rights")
                        .HasColumnType("int");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.AlbumPhotos", b =>
                {
                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int>("PhotoId")
                        .HasColumnType("int");

                    b.HasKey("AlbumId", "PhotoId");

                    b.HasIndex("PhotoId");

                    b.ToTable("AlbumPhotos");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.AlbumTags", b =>
                {
                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("AlbumId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("AlbumTags");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupAlbums", b =>
                {
                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.HasKey("AlbumId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupAlbums");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupInvitedUsers", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupInvitedUsers");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupRequestingUsers", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupRequestingUsers");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupUsers", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupUsers");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", "IconPhoto")
                        .WithMany()
                        .HasForeignKey("IconPhotoId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany("Albums")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("IconPhoto");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.GroupDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "Creator")
                        .WithMany("OwnedGroups")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", "IconPhoto")
                        .WithMany()
                        .HasForeignKey("IconPhotoId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Creator");

                    b.Navigation("IconPhoto");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.TagDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.AlbumPhotos", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Photo");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.AlbumTags", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.TagDTO", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupAlbums", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupInvitedUsers", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupRequestingUsers", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Persistence.RelationClasses.GroupUsers", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.UserDTO", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("OwnedGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
