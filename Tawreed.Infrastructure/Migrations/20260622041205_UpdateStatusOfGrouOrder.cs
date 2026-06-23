using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusOfGrouOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "group_order_events",
                keyColumn: "id",
                keyValue: new Guid("3e42bb9d-22dd-4a16-99c7-b5c481a92baf"),
                column: "event_type",
                value: "Closed");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae"),
                column: "status",
                value: "Closed");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("3d1335a9-a25c-4762-9f39-8046fc475f7a"),
                column: "status",
                value: "Open");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "group_order_events",
                keyColumn: "id",
                keyValue: new Guid("3e42bb9d-22dd-4a16-99c7-b5c481a92baf"),
                column: "event_type",
                value: "Locked");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae"),
                column: "status",
                value: "Locked");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("3d1335a9-a25c-4762-9f39-8046fc475f7a"),
                column: "status",
                value: "PendingApproval");
        }
    }
}
