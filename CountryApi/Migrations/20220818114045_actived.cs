using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CountryApi.Migrations
{
    public partial class actived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Currency",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Country",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Country");
        }
    }
}
