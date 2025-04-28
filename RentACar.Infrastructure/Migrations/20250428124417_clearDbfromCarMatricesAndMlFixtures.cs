using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class clearDbfromCarMatricesAndMlFixtures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key first
            migrationBuilder.DropForeignKey(
                name: "FK_CarMetrics_Cars_CarId",
                table: "CarMetrics");

            // Drop the CarMetrics table
            migrationBuilder.DropTable(
                name: "CarMetrics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate CarMetrics table
            migrationBuilder.CreateTable(
                name: "CarMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    LastServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EngineTemperature = table.Column<float>(type: "real", nullable: false),
                    OilLevel = table.Column<float>(type: "real", nullable: false),
                    TireWear = table.Column<float>(type: "real", nullable: false),
                    BrakeWear = table.Column<float>(type: "real", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarMetrics_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarMetrics_CarId",
                table: "CarMetrics",
                column: "CarId",
                unique: true);
        }
    }
}
