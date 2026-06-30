using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawreed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArabicNamesToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("005f9f49-e511-4130-8d38-9bfe78b50cd1"),
                column: "name",
                value: "سجق بقري 500 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("07070f15-a49e-49c7-a981-61fa8264550b"),
                column: "name",
                value: "فرخة كاملة طازجة 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("085218ae-4f81-4815-8428-33de12e3bef5"),
                column: "name",
                value: "زبدة المراعي 100 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0a869846-885f-4383-bc2b-01a987b1d719"),
                column: "name",
                value: "دمتي جبن مثلثات 250 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0da04f7e-a779-45dd-98e3-0558971e6dd7"),
                column: "name",
                value: "فانتا برتقال 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0e907b2b-b45d-4bad-833b-f29edab44fec"),
                column: "name",
                value: "شيبسي هوهوز كريمة حامضة 50 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("17f2af19-6ca1-402a-93f5-a08193696c07"),
                column: "name",
                value: "شيبسي هوهوز كاتشب 50 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("366a5f46-4aa5-4336-b419-456d64fca6d0"),
                column: "name",
                value: "كوكاكولا 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("45f06ed3-d5fa-4343-9d52-a87ecfa5bae1"),
                column: "name",
                value: "حليب جهينة نصف الدسم 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("4c9fa6f8-d251-466a-814f-1032d967e967"),
                column: "name",
                value: "عصير جهينة مشكل 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("571639f8-63d7-4715-9e12-34c2c35a4098"),
                column: "name",
                value: "بيبسي 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5ee00cf1-c0ad-43d7-a7fb-14da2f1c1034"),
                column: "name",
                value: "بسكو مصر ماري 200 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5ef8ef3b-6887-4a94-9241-2f61540b1383"),
                column: "name",
                value: "حليب دينا فارم 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5f815f2c-98fd-4070-a3b2-e48b8e3783b4"),
                column: "name",
                value: "دمتي موزاريلا 250 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("65f927b8-ded7-4ed6-9448-b0f28c48fb1f"),
                column: "name",
                value: "زبادي المراعي يوناني 200 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("75a1ba53-6f03-4e19-a309-9a48c942a457"),
                column: "name",
                value: "قشطة جهينة 200 مل");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("76e31cfe-371b-4241-af12-988ba88bad02"),
                column: "name",
                value: "قشطة دينا فارم 200 مل");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("76f4fd44-f497-4e75-96c9-b21f37d47e41"),
                column: "name",
                value: "سبرايت 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("888aae79-f1d4-4e2a-af83-e62792d8eff4"),
                column: "name",
                value: "حليب المراعي كامل الدسم 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("8b30e673-a339-49e9-8a73-d282b0339489"),
                column: "name",
                value: "برجر بقري 500 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("93999360-d133-42f3-a020-b3cbfb7d407a"),
                column: "name",
                value: "لحم ضأن مجمد 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("945caeac-565f-4260-b379-485b70aa0d7e"),
                column: "name",
                value: "لبن المراعي 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("a06d4bca-a8ec-4f2a-a19c-9978964fd086"),
                column: "name",
                value: "مول روزيتا بسكويت 100 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ace045a1-907c-4d76-939f-fd03d0e84a11"),
                column: "name",
                value: "حليب جهينة كامل الدسم 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b2bd0cdb-5ccb-4816-bb1d-795daf2d248f"),
                column: "name",
                value: "بيبسي كان 330 مل");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b7138f5f-4e7c-47e5-8f8e-7cd3108fb1a1"),
                column: "name",
                value: "بسكو مصر ويفر شوكولاتة 100 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b9654c58-a387-4fc1-aaf9-7dae7839638c"),
                column: "name",
                value: "7UP 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b9a84540-03bb-41f7-a039-3065484f7630"),
                column: "name",
                value: "أجنحة دجاج 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("bc5cd6ef-68f9-483c-8969-5df770449d62"),
                column: "name",
                value: "أوراك دجاج 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("bf01f346-7342-47e2-a3be-3ccadcb89f2a"),
                column: "name",
                value: "صدر دجاج 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c55cedf9-be8b-4aa6-ad96-1a2dd7c2415b"),
                column: "name",
                value: "فريسكا كيك 80 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c56d0e5c-19c1-4490-9e96-b541b16c7b67"),
                column: "name",
                value: "دمتي لبن بودرة 2 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c8b5ff65-f63f-4ba0-85ff-206b55a0cd70"),
                column: "name",
                value: "زبادي دينا فارم 500 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("cf09c9ba-8c7e-4d32-a9e4-f7f4c3ec2746"),
                column: "name",
                value: "زبادي جهينة فراولة 150 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d2fd9e66-456b-4773-851e-86c5a7bdd77c"),
                column: "name",
                value: "بسكو مصر بيتي بور 150 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d344ee05-027f-4d00-9f53-13fa205a7286"),
                column: "name",
                value: "لحم بقري مجمد 1 كجم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d3a8ec1a-b14a-447c-938f-195210354111"),
                column: "name",
                value: "دمتي جبن كريمي 200 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("dc0b12cb-7686-4d36-88ac-e721966fb65f"),
                column: "name",
                value: "جبن المراعي شيدر 250 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("e1e52a0f-1f22-4071-8562-e8df1c4e515e"),
                column: "name",
                value: "ميريندا برتقال 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("e5aec121-254e-42c9-a676-80453debbf89"),
                column: "name",
                value: "زبادي جهينة بلين 500 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ec7275f7-13c2-4305-849f-bd71660046d3"),
                column: "name",
                value: "كوكاكولا كان 330 مل");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ed9a09e8-3842-4bc8-bbd6-935f58b1b97a"),
                column: "name",
                value: "بسكو مصر شاي 200 جم");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f7b1cd26-efa7-4859-8ecf-95facdfcc4d4"),
                column: "name",
                value: "شويبس صودا 1 لتر");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f80382e9-94b5-4ca0-b356-232398badbdb"),
                column: "name",
                value: "لبن رايب جهينة 1 لتر");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("005f9f49-e511-4130-8d38-9bfe78b50cd1"),
                column: "name",
                value: "Sausage Beef 500g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("07070f15-a49e-49c7-a981-61fa8264550b"),
                column: "name",
                value: "Whole Chicken Fresh 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("085218ae-4f81-4815-8428-33de12e3bef5"),
                column: "name",
                value: "Almarai Butter 100g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0a869846-885f-4383-bc2b-01a987b1d719"),
                column: "name",
                value: "Domty Cheese Triangles 250g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0da04f7e-a779-45dd-98e3-0558971e6dd7"),
                column: "name",
                value: "Fanta Orange 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("0e907b2b-b45d-4bad-833b-f29edab44fec"),
                column: "name",
                value: "HOHOs Chips Sour Cream 50g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("17f2af19-6ca1-402a-93f5-a08193696c07"),
                column: "name",
                value: "HOHOs Chips Ketchup 50g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("366a5f46-4aa5-4336-b419-456d64fca6d0"),
                column: "name",
                value: "Coca-Cola 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("45f06ed3-d5fa-4343-9d52-a87ecfa5bae1"),
                column: "name",
                value: "Juhayna Milk Half Fat 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("4c9fa6f8-d251-466a-814f-1032d967e967"),
                column: "name",
                value: "Juhayna Juice Mixed 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("571639f8-63d7-4715-9e12-34c2c35a4098"),
                column: "name",
                value: "Pepsi 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5ee00cf1-c0ad-43d7-a7fb-14da2f1c1034"),
                column: "name",
                value: "Bisco Misr Marie Biscuit 200g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5ef8ef3b-6887-4a94-9241-2f61540b1383"),
                column: "name",
                value: "Dina Farms Milk 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("5f815f2c-98fd-4070-a3b2-e48b8e3783b4"),
                column: "name",
                value: "Domty Mozzarella 250g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("65f927b8-ded7-4ed6-9448-b0f28c48fb1f"),
                column: "name",
                value: "Almarai Yogurt Greek 200g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("75a1ba53-6f03-4e19-a309-9a48c942a457"),
                column: "name",
                value: "Juhayna Cream 200ml");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("76e31cfe-371b-4241-af12-988ba88bad02"),
                column: "name",
                value: "Dina Farms Cream 200ml");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("76f4fd44-f497-4e75-96c9-b21f37d47e41"),
                column: "name",
                value: "Sprite 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("888aae79-f1d4-4e2a-af83-e62792d8eff4"),
                column: "name",
                value: "Almarai Milk Full Cream 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("8b30e673-a339-49e9-8a73-d282b0339489"),
                column: "name",
                value: "Beef Burger 500g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("93999360-d133-42f3-a020-b3cbfb7d407a"),
                column: "name",
                value: "Frozen Lamb 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("945caeac-565f-4260-b379-485b70aa0d7e"),
                column: "name",
                value: "Almarai Laban 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("a06d4bca-a8ec-4f2a-a19c-9978964fd086"),
                column: "name",
                value: "Mole Rosetta Biscuit 100g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ace045a1-907c-4d76-939f-fd03d0e84a11"),
                column: "name",
                value: "Juhayna Milk Full Cream 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b2bd0cdb-5ccb-4816-bb1d-795daf2d248f"),
                column: "name",
                value: "Pepsi Can 330ml");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b7138f5f-4e7c-47e5-8f8e-7cd3108fb1a1"),
                column: "name",
                value: "Bisco Misr Wafers Chocolate 100g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b9654c58-a387-4fc1-aaf9-7dae7839638c"),
                column: "name",
                value: "7UP 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("b9a84540-03bb-41f7-a039-3065484f7630"),
                column: "name",
                value: "Chicken Wings 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("bc5cd6ef-68f9-483c-8969-5df770449d62"),
                column: "name",
                value: "Chicken Thighs 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("bf01f346-7342-47e2-a3be-3ccadcb89f2a"),
                column: "name",
                value: "Chicken Breast 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c55cedf9-be8b-4aa6-ad96-1a2dd7c2415b"),
                column: "name",
                value: "Fresca Cake 80g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c56d0e5c-19c1-4490-9e96-b541b16c7b67"),
                column: "name",
                value: "Domty Milk Powder 2kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("c8b5ff65-f63f-4ba0-85ff-206b55a0cd70"),
                column: "name",
                value: "Dina Farms Yogurt 500g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("cf09c9ba-8c7e-4d32-a9e4-f7f4c3ec2746"),
                column: "name",
                value: "Juhayna Yogurt Strawberry 150g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d2fd9e66-456b-4773-851e-86c5a7bdd77c"),
                column: "name",
                value: "Bisco Misr Petit Beurre 150g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d344ee05-027f-4d00-9f53-13fa205a7286"),
                column: "name",
                value: "Frozen Beef 1kg");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("d3a8ec1a-b14a-447c-938f-195210354111"),
                column: "name",
                value: "Domty Cream Cheese 200g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("dc0b12cb-7686-4d36-88ac-e721966fb65f"),
                column: "name",
                value: "Almarai Cheddar Cheese 250g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("e1e52a0f-1f22-4071-8562-e8df1c4e515e"),
                column: "name",
                value: "Mirinda Orange 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("e5aec121-254e-42c9-a676-80453debbf89"),
                column: "name",
                value: "Juhayna Yogurt Plain 500g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ec7275f7-13c2-4305-849f-bd71660046d3"),
                column: "name",
                value: "Coca-Cola Can 330ml");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("ed9a09e8-3842-4bc8-bbd6-935f58b1b97a"),
                column: "name",
                value: "Bisco Misr Tea Biscuit 200g");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f7b1cd26-efa7-4859-8ecf-95facdfcc4d4"),
                column: "name",
                value: "Schweppes Soda 1L");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("f80382e9-94b5-4ca0-b356-232398badbdb"),
                column: "name",
                value: "Juhayna Laban Rayeb 1L");
        }
    }
}
