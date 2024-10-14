using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeResetPasswordCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForgetPasswordCode",
                table: "Users",
                newName: "ResetPasswordCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetPasswordCode",
                table: "Users",
                newName: "ForgetPasswordCode");
        }
    }
}
