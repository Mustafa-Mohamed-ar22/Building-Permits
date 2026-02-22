using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building_Permits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Client_one_Permit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permits_Clients_ClientId",
                table: "Permits");

            migrationBuilder.DropIndex(
                name: "IX_Permits_ClientId",
                table: "Permits");

            migrationBuilder.AddColumn<int>(
                name: "PermitId",
                table: "Clients",
                type: "int",
                nullable: true);


            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Permits_PermitId",
                table: "Clients",
                column: "PermitId",
                principalTable: "Permits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Permits_PermitId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_PermitId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PermitId",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_Permits_ClientId",
                table: "Permits",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permits_Clients_ClientId",
                table: "Permits",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
