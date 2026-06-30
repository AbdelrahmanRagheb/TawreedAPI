using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificationToGroupOrderAndGroupOrderItemTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "item_status",
                table: "group_order_items",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Unassigned");

            migrationBuilder.AddColumn<Guid>(
                name: "supplier_id",
                table: "group_order_items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("1e1cfe17-cc86-43ab-b7ff-b27bb1990a21"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("334c1051-ab0a-41e4-8522-08a717723e57"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("5481a4c0-9bff-4aaa-a059-bdb3702631fb"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("723a6ef4-d0e4-4640-8631-c55ee207ca25"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("7c4cc6ff-87e3-4dc8-8bc8-dec4466f8a30"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("ae7c6cdc-a80b-482e-944b-63a073d4f8f5"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.UpdateData(
                table: "group_order_items",
                keyColumn: "id",
                keyValue: new Guid("bcb1ab44-59e5-4fd4-8540-ca210f2c2af5"),
                columns: new[] { "item_status", "supplier_id" },
                values: new object[] { "Unassigned", null });

            migrationBuilder.CreateIndex(
                name: "ix_group_order_items_supplier_id",
                table: "group_order_items",
                column: "supplier_id");

            migrationBuilder.AddForeignKey(
                name: "fk_group_order_items_suppliers_supplier_id",
                table: "group_order_items",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_order_items_suppliers_supplier_id",
                table: "group_order_items");

            migrationBuilder.DropIndex(
                name: "ix_group_order_items_supplier_id",
                table: "group_order_items");

            migrationBuilder.DropColumn(
                name: "item_status",
                table: "group_order_items");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "group_order_items");
        }
    }
}
