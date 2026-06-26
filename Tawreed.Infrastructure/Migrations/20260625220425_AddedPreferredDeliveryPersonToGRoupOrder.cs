using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPreferredDeliveryPersonToGRoupOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_group_orders_preferred_delivery_person_id",
                table: "group_orders",
                column: "preferred_delivery_person_id");

            migrationBuilder.AddForeignKey(
                name: "fk_group_orders_delivery_person_profiles_preferred_delivery_person_id",
                table: "group_orders",
                column: "preferred_delivery_person_id",
                principalTable: "delivery_person_profiles",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_group_orders_delivery_person_profiles_preferred_delivery_person_id",
                table: "group_orders");

            migrationBuilder.DropIndex(
                name: "ix_group_orders_preferred_delivery_person_id",
                table: "group_orders");
        }
    }
}
