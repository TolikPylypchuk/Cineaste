using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cineaste.Persistence.Migrations
{
    public partial class AddTestListsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lists",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("04071a9e-17ed-4aa0-855e-7c89ed16921b"), "Test List 3" },
                    { new Guid("3770d3f7-47b3-4865-9819-11268c9b965f"), "Test List 2" },
                    { new Guid("45db8370-09d6-43bf-80e5-6818175189d6"), "Test List 4" },
                    { new Guid("c7c2dc17-dbb0-4506-989a-30a7904fd5ba"), "Test List 5" },
                    { new Guid("f93ab0d0-189b-4806-850f-e129bd5af30a"), "Test List 1" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: new Guid("04071a9e-17ed-4aa0-855e-7c89ed16921b"));

            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: new Guid("3770d3f7-47b3-4865-9819-11268c9b965f"));

            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: new Guid("45db8370-09d6-43bf-80e5-6818175189d6"));

            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: new Guid("c7c2dc17-dbb0-4506-989a-30a7904fd5ba"));

            migrationBuilder.DeleteData(
                table: "Lists",
                keyColumn: "Id",
                keyValue: new Guid("f93ab0d0-189b-4806-850f-e129bd5af30a"));
        }
    }
}
