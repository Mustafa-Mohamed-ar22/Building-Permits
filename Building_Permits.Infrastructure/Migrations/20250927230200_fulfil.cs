using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building_Permits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fulfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FulfilmentDate",
                table: "Permits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPaused",
                table: "Permits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FulfilmentDate",
                table: "Permits");

            migrationBuilder.DropColumn(
                name: "isPaused",
                table: "Permits");
        }
    }
}
