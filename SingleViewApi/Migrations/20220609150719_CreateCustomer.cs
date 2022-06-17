using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ServiceStack.DataAnnotations;

#nullable disable

namespace SingleViewApi.V1.Infrastructure.Migrations
{
    public partial class CreateCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    ni_number = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_id",
                table: "customers",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_first_name",
                table: "customers",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "ix_customers_last_name",
                table: "customers",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "ix_customers_date_of_birth",
                table: "customers",
                column: "date_of_birth");

            migrationBuilder.CreateIndex(
                name: "ix_customers_ni_number",
                table: "customers",
                column: "ni_number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers");
        }
    }
}
