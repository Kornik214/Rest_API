using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scooters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    model = table.Column<string>(type: "varchar(100)", nullable: false),
                    serial_number = table.Column<string>(type: "varchar(50)", nullable: false),
                    battery_percent = table.Column<int>(type: "integer", nullable: false),
                    price_per_minute = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    last_service_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scooters", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_scooters_serial_number",
                table: "scooters",
                column: "serial_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scooters");
        }
    }
}
