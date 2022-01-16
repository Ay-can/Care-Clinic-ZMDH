using Microsoft.EntityFrameworkCore.Migrations;

namespace Wdpr_Groep_E.Migrations
{
    public partial class nogeen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "AspNetUsers");
        }
    }
}
