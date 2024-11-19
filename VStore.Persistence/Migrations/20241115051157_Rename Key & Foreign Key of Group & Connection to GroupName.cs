using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameKeyForeignKeyofGroupConnectiontoGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Groups_GroupId",
                table: "Connections");

            migrationBuilder.DropPrimaryKey(
                name: "GroupName",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Connections_GroupId",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Connections");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Groups",
                newName: "GroupName");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Connections",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_GroupName",
                table: "Connections",
                column: "GroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Groups_GroupName",
                table: "Connections",
                column: "GroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Groups_GroupName",
                table: "Connections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Connections_GroupName",
                table: "Connections");

            migrationBuilder.RenameColumn(
                name: "GroupName",
                table: "Groups",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Connections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Connections",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "GroupName",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_GroupId",
                table: "Connections",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Groups_GroupId",
                table: "Connections",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
