using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building_Permits.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isChecked",
                table: "PermitStages",
                newName: "isCheckedForOverDue");

            migrationBuilder.AddColumn<bool>(
                name: "isCheckedForNeglect",
                table: "PermitStages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCheckedForNeglect",
                table: "PermitStages");

            migrationBuilder.RenameColumn(
                name: "isCheckedForOverDue",
                table: "PermitStages",
                newName: "isChecked");
        }
    }
}
