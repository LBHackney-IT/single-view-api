using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SingleViewApi.V1.Infrastructure.Migrations
{
    public partial class DataSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_source",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_source", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_data_source_id",
                table: "data_source",
                column: "id");

            migrationBuilder.InsertData("data_source", "name", "HousingSearch");
            migrationBuilder.InsertData(
                table: "data_source",
                columns: new[] { "name" },
                values: new object[,]
                {
                    { "PersonAPI" },
                    { "Jigsaw" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_source");
        }
    }
}
