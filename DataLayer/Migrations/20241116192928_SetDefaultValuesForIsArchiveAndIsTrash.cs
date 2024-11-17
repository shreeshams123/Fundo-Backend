using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class SetDefaultValuesForIsArchiveAndIsTrash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsArchive",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false, 
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrash",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false, 
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsArchive",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false, 
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrash",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false, 
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
