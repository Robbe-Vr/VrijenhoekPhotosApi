using Microsoft.EntityFrameworkCore.Migrations;

namespace VrijenhoekPhotos.Persistence.Migrations
{
    public partial class groupids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumDTOGroupDTO_Groups_GroupsGroupName",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO_Groups_GroupsGroupName",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO1_Groups_PendingJoinGroupsGroupName",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO2_Groups_RequestedJoinGroupsGroupName",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO2",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO1",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumDTOGroupDTO",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsGroupName",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropColumn(
                name: "RequestedJoinGroupsGroupName",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropColumn(
                name: "PendingJoinGroupsGroupName",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropColumn(
                name: "GroupsGroupName",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropColumn(
                name: "GroupsGroupName",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "RequestedJoinGroupsId",
                table: "GroupDTOUserDTO2",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PendingJoinGroupsId",
                table: "GroupDTOUserDTO1",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupsId",
                table: "GroupDTOUserDTO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupsId",
                table: "AlbumDTOGroupDTO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO2",
                table: "GroupDTOUserDTO2",
                columns: new[] { "RequestedJoinGroupsId", "RequestedJoinUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO1",
                table: "GroupDTOUserDTO1",
                columns: new[] { "PendingJoinGroupsId", "PendingJoinUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO",
                table: "GroupDTOUserDTO",
                columns: new[] { "GroupsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumDTOGroupDTO",
                table: "AlbumDTOGroupDTO",
                columns: new[] { "AlbumsId", "GroupsId" });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsId",
                table: "AlbumDTOGroupDTO",
                column: "GroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumDTOGroupDTO_Groups_GroupsId",
                table: "AlbumDTOGroupDTO",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO_Groups_GroupsId",
                table: "GroupDTOUserDTO",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO1_Groups_PendingJoinGroupsId",
                table: "GroupDTOUserDTO1",
                column: "PendingJoinGroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO2_Groups_RequestedJoinGroupsId",
                table: "GroupDTOUserDTO2",
                column: "RequestedJoinGroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumDTOGroupDTO_Groups_GroupsId",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO_Groups_GroupsId",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO1_Groups_PendingJoinGroupsId",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupDTOUserDTO2_Groups_RequestedJoinGroupsId",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO2",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO1",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupDTOUserDTO",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumDTOGroupDTO",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsId",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RequestedJoinGroupsId",
                table: "GroupDTOUserDTO2");

            migrationBuilder.DropColumn(
                name: "PendingJoinGroupsId",
                table: "GroupDTOUserDTO1");

            migrationBuilder.DropColumn(
                name: "GroupsId",
                table: "GroupDTOUserDTO");

            migrationBuilder.DropColumn(
                name: "GroupsId",
                table: "AlbumDTOGroupDTO");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Groups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedJoinGroupsGroupName",
                table: "GroupDTOUserDTO2",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PendingJoinGroupsGroupName",
                table: "GroupDTOUserDTO1",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupsGroupName",
                table: "GroupDTOUserDTO",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupsGroupName",
                table: "AlbumDTOGroupDTO",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "GroupName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO2",
                table: "GroupDTOUserDTO2",
                columns: new[] { "RequestedJoinGroupsGroupName", "RequestedJoinUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO1",
                table: "GroupDTOUserDTO1",
                columns: new[] { "PendingJoinGroupsGroupName", "PendingJoinUsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupDTOUserDTO",
                table: "GroupDTOUserDTO",
                columns: new[] { "GroupsGroupName", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumDTOGroupDTO",
                table: "AlbumDTOGroupDTO",
                columns: new[] { "AlbumsId", "GroupsGroupName" });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumDTOGroupDTO_GroupsGroupName",
                table: "AlbumDTOGroupDTO",
                column: "GroupsGroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumDTOGroupDTO_Groups_GroupsGroupName",
                table: "AlbumDTOGroupDTO",
                column: "GroupsGroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO_Groups_GroupsGroupName",
                table: "GroupDTOUserDTO",
                column: "GroupsGroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO1_Groups_PendingJoinGroupsGroupName",
                table: "GroupDTOUserDTO1",
                column: "PendingJoinGroupsGroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupDTOUserDTO2_Groups_RequestedJoinGroupsGroupName",
                table: "GroupDTOUserDTO2",
                column: "RequestedJoinGroupsGroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
