using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_settings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_app_settings_key",
                table: "app_settings",
                column: "key",
                unique: true);

            migrationBuilder.InsertData(
                table: "app_settings",
                columns: new[] { "id", "key", "value", "description" },
                values: new object[]
                {
                    new Guid("11111111-1111-1111-1111-111111111111"),
                    "GroupRegionTypes",
                    "[\"Governorate\",\"Qism\",\"Markaz\",\"Madina\",\"Hayy\",\"PoliceDepartment\",\"Region\"]",
                    "Region types that can host group orders"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_settings");
        }
    }
}
