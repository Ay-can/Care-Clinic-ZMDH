using Microsoft.EntityFrameworkCore.Migrations;

namespace Wdpr_Groep_E.Migrations
{
    public partial class Leeftijdaanzelfhulpgroep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeGroup",
                table: "Chats",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeGroup",
                table: "Chats");
        }
    }
}
