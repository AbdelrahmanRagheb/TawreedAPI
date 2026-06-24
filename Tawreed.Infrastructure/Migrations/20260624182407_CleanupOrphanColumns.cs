using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupOrphanColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop orphan columns from deliveries table
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'shipping_latitude')
                    ALTER TABLE [deliveries] DROP COLUMN [shipping_latitude];

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'shipping_longitude')
                    ALTER TABLE [deliveries] DROP COLUMN [shipping_longitude];
            ");

            migrationBuilder.Sql(@"
                -- Drop orphan columns from invoices table
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_latitude')
                    ALTER TABLE [invoices] DROP COLUMN [shipping_latitude];

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_longitude')
                    ALTER TABLE [invoices] DROP COLUMN [shipping_longitude];

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'discount_amount')
                    ALTER TABLE [invoices] DROP COLUMN [discount_amount];

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'issued_at')
                    ALTER TABLE [invoices] DROP COLUMN [issued_at];
            ");

            migrationBuilder.Sql(@"
                -- Rename shipping_region to shipping_address on invoices (if needed)
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_region')
                   AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_address')
                    EXEC sp_rename 'invoices.shipping_region', 'shipping_address', 'COLUMN';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Reverse the shipping_region rename on invoices (if it was renamed)
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_address')
                   AND NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'invoices' AND COLUMN_NAME = 'shipping_region')
                    EXEC sp_rename 'invoices.shipping_address', 'shipping_region', 'COLUMN';
            ");
        }
    }
}
