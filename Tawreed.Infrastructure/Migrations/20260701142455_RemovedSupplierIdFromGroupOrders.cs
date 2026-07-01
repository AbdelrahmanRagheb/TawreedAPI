using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSupplierIdFromGroupOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_orders_suppliers_supplier_id",
                table: "group_orders");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("382e6a64-ea3a-4dcc-8c05-379aa01a3461"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("3d1335a9-a25c-4762-9f39-8046fc475f7a"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("6b1ac916-146a-44e8-a523-7d69d833f055"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("7e0d7a95-b9f5-40d9-beaf-4ac077593cf5"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("85085d3d-4938-4ba1-8b6a-2ef99f9995b0"),
                column: "supplier_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("bba56c6a-827e-4394-a10a-b81c6d9dbeaf"),
                column: "supplier_id",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "fk_group_orders_suppliers_supplier_id",
                table: "group_orders",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_orders_suppliers_supplier_id",
                table: "group_orders");

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("382e6a64-ea3a-4dcc-8c05-379aa01a3461"),
                column: "supplier_id",
                value: new Guid("40c42c46-3774-4ee0-ad5f-d6b16c8c0f6f"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae"),
                column: "supplier_id",
                value: new Guid("59925002-c1a0-4489-b279-943f1269819e"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("3d1335a9-a25c-4762-9f39-8046fc475f7a"),
                column: "supplier_id",
                value: new Guid("590d8027-2ace-4e51-b627-9a10c5fcfce1"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("6b1ac916-146a-44e8-a523-7d69d833f055"),
                column: "supplier_id",
                value: new Guid("5374892c-cf76-4192-ad6a-f7142bcb1842"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("7e0d7a95-b9f5-40d9-beaf-4ac077593cf5"),
                column: "supplier_id",
                value: new Guid("d3d792d0-2e84-4415-8345-90cf3eef943f"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("85085d3d-4938-4ba1-8b6a-2ef99f9995b0"),
                column: "supplier_id",
                value: new Guid("d967d2d6-a707-430f-b404-9b9b5858086e"));

            migrationBuilder.UpdateData(
                table: "group_orders",
                keyColumn: "id",
                keyValue: new Guid("bba56c6a-827e-4394-a10a-b81c6d9dbeaf"),
                column: "supplier_id",
                value: new Guid("d6a1a124-f64a-4b4f-b7cd-03642969e000"));

            migrationBuilder.AddForeignKey(
                name: "fk_group_orders_suppliers_supplier_id",
                table: "group_orders",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
