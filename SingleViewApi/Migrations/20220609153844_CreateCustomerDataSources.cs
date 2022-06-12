using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SingleViewApi.V1.Infrastructure.Migrations
{
    public partial class CreateCustomerDataSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_data_sources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<int>(type: "uuid", nullable: false),
                    data_source_id = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_data_sources", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_data_sources_id",
                table: "customer_data_sources",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_data_sources_customer_id",
                table: "customer_data_sources",
                column: "customer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_data_sources");
        }
    }
}
