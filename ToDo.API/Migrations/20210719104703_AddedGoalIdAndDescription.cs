using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDo.API.Migrations
{
    public partial class AddedGoalIdAndDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "goal_description",
                table: "todo",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "goal_id",
                table: "todo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "goal_description",
                table: "todo");

            migrationBuilder.DropColumn(
                name: "goal_id",
                table: "todo");
        }
    }
}
