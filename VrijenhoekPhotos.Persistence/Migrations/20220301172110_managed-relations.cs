using Microsoft.EntityFrameworkCore.Migrations;

namespace VrijenhoekPhotos.Persistence.Migrations
{
    public partial class managedrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "AlbumPhotos",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    PhotoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumPhotos", x => new { x.AlbumId, x.PhotoId });
                    table.ForeignKey(
                        name: "FK_AlbumPhotos_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumPhotos_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumTags",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTags", x => new { x.AlbumId, x.TagId });
                    table.ForeignKey(
                        name: "FK_AlbumTags_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAlbums",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAlbums", x => new { x.AlbumId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_GroupAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAlbums_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupInvitedUsers",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvitedUsers", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupInvitedUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupInvitedUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRequestingUsers",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRequestingUsers", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupRequestingUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRequestingUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUsers",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUsers", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupUsers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumPhotos_PhotoId",
                table: "AlbumPhotos",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTags_TagId",
                table: "AlbumTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAlbums_GroupId",
                table: "GroupAlbums",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitedUsers_UserId",
                table: "GroupInvitedUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRequestingUsers_UserId",
                table: "GroupRequestingUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUsers_UserId",
                table: "GroupUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumPhotos");

            migrationBuilder.DropTable(
                name: "AlbumTags");

            migrationBuilder.DropTable(
                name: "GroupAlbums");

            migrationBuilder.DropTable(
                name: "GroupInvitedUsers");

            migrationBuilder.DropTable(
                name: "GroupRequestingUsers");

            migrationBuilder.DropTable(
                name: "GroupUsers");

            migrationBuilder.CreateTable(
                name: "AlbumDTOGroupDTO",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "int", nullable: false),
                    GroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumDTOGroupDTO", x => new { x.AlbumsId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_AlbumDTOGroupDTO_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumDTOGroupDTO_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumDTOPhotoDTO",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "int", nullable: false),
                    PhotosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumDTOPhotoDTO", x => new { x.AlbumsId, x.PhotosId });
                    table.ForeignKey(
                        name: "FK_AlbumDTOPhotoDTO_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumDTOPhotoDTO_Photos_PhotosId",
                        column: x => x.PhotosId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumDTOTagDTO",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumDTOTagDTO", x => new { x.AlbumsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_AlbumDTOTagDTO_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumDTOTagDTO_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupDTOUserDTO",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDTOUserDTO", x => new { x.GroupsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupDTOUserDTO1",
                columns: table => new
                {
                    PendingJoinGroupsId = table.Column<int>(type: "int", nullable: false),
                    PendingJoinUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDTOUserDTO1", x => new { x.PendingJoinGroupsId, x.PendingJoinUsersId });
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO1_Groups_PendingJoinGroupsId",
                        column: x => x.PendingJoinGroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO1_Users_PendingJoinUsersId",
                        column: x => x.PendingJoinUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupDTOUserDTO2",
                columns: table => new
                {
                    RequestedJoinGroupsId = table.Column<int>(type: "int", nullable: false),
                    RequestedJoinUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupDTOUserDTO2", x => new { x.RequestedJoinGroupsId, x.RequestedJoinUsersId });
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO2_Groups_RequestedJoinGroupsId",
                        column: x => x.RequestedJoinGroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupDTOUserDTO2_Users_RequestedJoinUsersId",
                        column: x => x.RequestedJoinUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsId",
                table: "AlbumDTOGroupDTO",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOPhotoDTO_PhotosId",
                table: "AlbumDTOPhotoDTO",
                column: "PhotosId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOTagDTO_TagsId",
                table: "AlbumDTOTagDTO",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDTOUserDTO_UsersId",
                table: "GroupDTOUserDTO",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDTOUserDTO1_PendingJoinUsersId",
                table: "GroupDTOUserDTO1",
                column: "PendingJoinUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupDTOUserDTO2_RequestedJoinUsersId",
                table: "GroupDTOUserDTO2",
                column: "RequestedJoinUsersId");
        }
    }
}
