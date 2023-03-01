using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AJProds.EFDataSeeder.Tests.Common.Migrations.PostgreSQL;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
                                      name: "tst");

        migrationBuilder.CreateTable(
                                     name: "Testees",
                                     schema: "tst",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<int>(type: "integer", nullable: false)
                                                                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                                           Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_Testees", x => x.Id);
                                                  });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   name: "Testees",
                                   schema: "tst");
    }
}