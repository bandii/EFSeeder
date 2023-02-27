using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AJProds.EFDataSeeder.PostgreSql.Migrations
{
    public partial class Initial : Migration
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstRunAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SeedName = table.Column<string>(type: "text", nullable: false),
                    AlwaysRun = table.Column<bool>(type: "boolean", nullable: false)
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
