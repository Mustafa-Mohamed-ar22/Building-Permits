using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building_Permits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class reason2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PausedAt",
                table: "Permits",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PausedAt",
                table: "Permits");
        }
    }
}
