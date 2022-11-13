using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class newPlayerStat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Speed",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "TotalExp",
                table: "Player");

            migrationBuilder.AlterColumn<float>(
                name: "MaxHp",
                table: "Player",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Hp",
                table: "Player",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Attack",
                table: "Player",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "AttackSpeed",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Current_exp",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Defence",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Exp",
                table: "Player",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Gold",
                table: "Player",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<float>(
                name: "Mana",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Mana_regeneration",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Max_mana",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MoveSpeed",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Next_level_up",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Regeneration",
                table: "Player",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Skill_point",
                table: "Player",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TurnSpeed",
                table: "Player",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttackSpeed",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Current_exp",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Defence",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Exp",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Gold",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Mana",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Mana_regeneration",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Max_mana",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "MoveSpeed",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Next_level_up",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Regeneration",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Skill_point",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "TurnSpeed",
                table: "Player");

            migrationBuilder.AlterColumn<int>(
                name: "MaxHp",
                table: "Player",
                type: "int",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "Hp",
                table: "Player",
                type: "int",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "Attack",
                table: "Player",
                type: "int",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AddColumn<float>(
                name: "Speed",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "TotalExp",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
