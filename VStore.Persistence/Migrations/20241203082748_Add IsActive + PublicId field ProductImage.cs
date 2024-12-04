using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActivePublicIdfieldProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductImages");
        }
    }
}
