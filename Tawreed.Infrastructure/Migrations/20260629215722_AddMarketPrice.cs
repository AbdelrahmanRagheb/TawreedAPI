using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "market_price",
                table: "products",
                type: "decimal(12,2)",
                nullable: true);

            // Set initial market prices for seeded products
            // Juhayna products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 45 WHERE name LIKE 'Juhayna Milk%1L'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 15 WHERE name LIKE 'Juhayna Yogurt Plain%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 8 WHERE name LIKE 'Juhayna Yogurt Strawberry%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 30 WHERE name LIKE 'Juhayna Juice%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 35 WHERE name LIKE 'Juhayna Laban%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 20 WHERE name LIKE 'Juhayna Cream%'");

            // Almarai products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 48 WHERE name LIKE 'Almarai Milk%1L'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 18 WHERE name LIKE 'Almarai Yogurt%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 25 WHERE name LIKE 'Almarai Butter%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 40 WHERE name LIKE 'Almarai Cheddar%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 38 WHERE name LIKE 'Almarai Laban%'");

            // Coca-Cola products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 15 WHERE name IN ('Coca-Cola 1L', 'Fanta Orange 1L', 'Sprite 1L')");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 18 WHERE name = 'Schweppes Soda 1L'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 8 WHERE name = 'Coca-Cola Can 330ml'");

            // Pepsi products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 14 WHERE name IN ('Pepsi 1L', '7UP 1L', 'Mirinda Orange 1L')");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 7 WHERE name = 'Pepsi Can 330ml'");

            // Bisco Misr products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 12 WHERE name LIKE 'Bisco Misr Tea Biscuit%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 10 WHERE name LIKE 'Bisco Misr Petit Beurre%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 8 WHERE name LIKE 'Bisco Misr Wafers%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 10 WHERE name LIKE 'Bisco Misr Marie%'");

            // Domty products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 35 WHERE name LIKE 'Domty Cheese Triangles%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 30 WHERE name LIKE 'Domty Cream Cheese%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 38 WHERE name LIKE 'Domty Mozzarella%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 180 WHERE name LIKE 'Domty Milk Powder%'");

            // Edita products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 6 WHERE name LIKE 'HOHOs Chips%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 8 WHERE name LIKE 'Mole Rosetta%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 10 WHERE name LIKE 'Fresca Cake%'");

            // Selsela poultry products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 90 WHERE name = 'Whole Chicken Fresh 1kg'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 120 WHERE name = 'Chicken Breast 1kg'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 80 WHERE name = 'Chicken Thighs 1kg'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 60 WHERE name = 'Chicken Wings 1kg'");

            // Fath frozen products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 220 WHERE name = 'Frozen Beef 1kg'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 280 WHERE name = 'Frozen Lamb 1kg'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 80 WHERE name LIKE 'Beef Burger%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 60 WHERE name LIKE 'Sausage Beef%'");

            // Dina Farms products
            migrationBuilder.Sql(@"UPDATE products SET market_price = 42 WHERE name LIKE 'Dina Farms Milk%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 14 WHERE name LIKE 'Dina Farms Yogurt%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 18 WHERE name LIKE 'Dina Farms Cream%'");
            migrationBuilder.Sql(@"UPDATE products SET market_price = 34 WHERE name LIKE 'Dina Farms Laban%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "market_price",
                table: "products");
        }
    }
}
