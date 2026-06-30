using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotesAndVisibilityFromGroupOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notes",
                table: "group_orders");

            migrationBuilder.DropColumn(
                name: "visibility",
                table: "group_orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "group_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "visibility",
                table: "group_orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("382e6a64-ea3a-4dcc-8c05-379aa01a3461"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("3d1335a9-a25c-4762-9f39-8046fc475f7a"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("6b1ac916-146a-44e8-a523-7d69d833f055"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("7e0d7a95-b9f5-40d9-beaf-4ac077593cf5"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("85085d3d-4938-4ba1-8b6a-2ef99f9995b0"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("bba56c6a-827e-4394-a10a-b81c6d9dbeaf"),
                columns: new[] { "notes", "visibility" },
                values: new object[] { null, null });
        }
    }
}
