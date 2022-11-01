using Microsoft.EntityFrameworkCore.Migrations;

namespace SharedDB.Migrations
{
    public partial class Open : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Open",
                table: "ServerInfo",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Open",
                table: "ServerInfo");
        }
    }
}
