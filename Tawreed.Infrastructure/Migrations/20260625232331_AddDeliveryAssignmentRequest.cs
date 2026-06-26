using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAssignmentRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delivery_assignment_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    delivery_person_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    supplier_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    proposed_fee = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    responded_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    decline_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_assignment_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_delivery_assignment_requests_delivery_person_profiles_delivery_person_id",
                        column: x => x.delivery_person_id,
                        principalTable: "delivery_person_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_delivery_assignment_requests_group_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "group_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_delivery_assignment_requests_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_assignment_requests_delivery_person_id",
                table: "delivery_assignment_requests",
                column: "delivery_person_id");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_assignment_requests_order_id_delivery_person_id",
                table: "delivery_assignment_requests",
                columns: new[] { "order_id", "delivery_person_id" });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_assignment_requests_status",
                table: "delivery_assignment_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_assignment_requests_supplier_id",
                table: "delivery_assignment_requests",
                column: "supplier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_assignment_requests");
        }
    }
}
