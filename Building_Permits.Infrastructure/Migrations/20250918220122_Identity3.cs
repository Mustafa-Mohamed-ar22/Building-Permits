using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building_Permits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Identity3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Permits",
                type: "nvarchar(450)",
                nullable: true,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Permits_CreatedBy",
                table: "Permits",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Permits_AspNetUsers_CreatedBy",
                table: "Permits",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permits_AspNetUsers_CreatedBy",
                table: "Permits");

            migrationBuilder.DropIndex(
                name: "IX_Permits_CreatedBy",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Permits");
        }
    }
}
