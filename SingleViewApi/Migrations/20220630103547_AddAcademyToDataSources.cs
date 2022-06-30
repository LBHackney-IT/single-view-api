using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SingleViewApi.V1.Infrastructure.Migrations
{
    public partial class UpdateDataSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "data_sources",
                columns: new[] { "name" },
                values: new object[,]
                {
                    { "Academy-CouncilTax" },
                    { "Academy-Benefits" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
