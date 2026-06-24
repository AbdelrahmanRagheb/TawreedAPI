using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeliveryVerificationCodeToDelvieryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "verification_code",
                table: "deliveries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verification_code",
                table: "deliveries");
        }
    }
}
