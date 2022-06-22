using Microsoft.EntityFrameworkCore.Migrations;

namespace VrijenhoekPhotos.Persistence.Migrations
{
    public partial class groupswithalbums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Albums_AlbumDTOId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_AlbumDTOId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AlbumDTOId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "IconPhotoId",
                table: "Groups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IconPhotoId",
                table: "Albums",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "AlbumDTOGroupDTO",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "int", nullable: false),
                    GroupsGroupName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumDTOGroupDTO", x => new { x.AlbumsId, x.GroupsGroupName });
                    table.ForeignKey(
                        name: "FK_AlbumDTOGroupDTO_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumDTOGroupDTO_Groups_GroupsGroupName",
                        column: x => x.GroupsGroupName,
                        principalTable: "Groups",
                        principalColumn: "GroupName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsGroupName",
                table: "AlbumDTOGroupDTO",
                column: "GroupsGroupName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumDTOGroupDTO");

            migrationBuilder.AlterColumn<int>(
                name: "IconPhotoId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlbumDTOId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IconPhotoId",
                table: "Albums",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AlbumDTOId",
                table: "Groups",
                column: "AlbumDTOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Albums_AlbumDTOId",
                table: "Groups",
                column: "AlbumDTOId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
