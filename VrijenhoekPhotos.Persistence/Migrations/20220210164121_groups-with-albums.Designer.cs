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
    [Migration("20220210164121_groups-with-albums")]
    partial class groupswithalbums
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AlbumDTOGroupDTO", b =>
                {
                    b.Property<int>("AlbumsId")
                        .HasColumnType("int");

                    b.Property<string>("GroupsGroupName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AlbumsId", "GroupsGroupName");

                    b.HasIndex("GroupsGroupName");

                    b.ToTable("AlbumDTOGroupDTO");
                });

            modelBuilder.Entity("AlbumDTOPhotoDTO", b =>
                {
                    b.Property<int>("AlbumsId")
                        .HasColumnType("int");

                    b.Property<int>("PhotosId")
                        .HasColumnType("int");

                    b.HasKey("AlbumsId", "PhotosId");

                    b.HasIndex("PhotosId");

                    b.ToTable("AlbumDTOPhotoDTO");
                });

            modelBuilder.Entity("GroupDTOUserDTO", b =>
                {
                    b.Property<string>("GroupsGroupName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UsersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GroupsGroupName", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("GroupDTOUserDTO");
                });

            modelBuilder.Entity("GroupDTOUserDTO1", b =>
                {
                    b.Property<string>("PendingJoinGroupsGroupName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PendingJoinUsersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PendingJoinGroupsGroupName", "PendingJoinUsersId");

                    b.HasIndex("PendingJoinUsersId");

                    b.ToTable("GroupDTOUserDTO1");
                });

            modelBuilder.Entity("GroupDTOUserDTO2", b =>
                {
                    b.Property<string>("RequestedJoinGroupsGroupName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RequestedJoinUsersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RequestedJoinGroupsGroupName", "RequestedJoinUsersId");

                    b.HasIndex("RequestedJoinUsersId");

                    b.ToTable("GroupDTOUserDTO2");
                });

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
                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("IconPhotoId")
                        .HasColumnType("int");

                    b.HasKey("GroupName");

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

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.UserDTO", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AlbumDTOGroupDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", null)
                        .WithMany()
                        .HasForeignKey("AlbumsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", null)
                        .WithMany()
                        .HasForeignKey("GroupsGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlbumDTOPhotoDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.AlbumDTO", null)
                        .WithMany()
                        .HasForeignKey("AlbumsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.PhotoDTO", null)
                        .WithMany()
                        .HasForeignKey("PhotosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupDTOUserDTO", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", null)
                        .WithMany()
                        .HasForeignKey("GroupsGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupDTOUserDTO1", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", null)
                        .WithMany()
                        .HasForeignKey("PendingJoinGroupsGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", null)
                        .WithMany()
                        .HasForeignKey("PendingJoinUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GroupDTOUserDTO2", b =>
                {
                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.GroupDTO", null)
                        .WithMany()
                        .HasForeignKey("RequestedJoinGroupsGroupName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VrijenhoekPhotos.Exchange.Classes.UserDTO", null)
                        .WithMany()
                        .HasForeignKey("RequestedJoinUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("VrijenhoekPhotos.Exchange.Classes.UserDTO", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("OwnedGroups");
                });
#pragma warning restore 612, 618
        }
    }
}