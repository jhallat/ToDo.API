using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDo.API.Migrations
{
    public partial class UpdateQuantityNameOnToDo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "todo",
                newName: "quantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "todo",
                newName: "Quantity");
        }
    }
}
