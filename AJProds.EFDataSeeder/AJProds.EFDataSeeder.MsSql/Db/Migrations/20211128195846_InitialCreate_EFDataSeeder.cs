using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AJProds.EFDataSeeder.MsSql.Db.Migrations
{
    public partial class InitialCreate_EFDataSeeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sdr");

            migrationBuilder.CreateTable(
                name: "SeederHistories",
                schema: "sdr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstRunAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlwaysRun = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeederHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeederHistories_SeedName",
                schema: "sdr",
                table: "SeederHistories",
                column: "SeedName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeederHistories",
                schema: "sdr");
        }
    }
}
