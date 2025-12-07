using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urunsatisportali.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(352), "Elektronik ürünler", "Elektronik" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(354), "Giyim ve aksesuar", "Giyim" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(356), "Yiyecek ve içecek ürünleri", "Yiyecek & İçecek" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(358), "Ev ve bahçe ürünleri", "Ev & Bahçe" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "TechMarka", new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(545), "Yüksek performanslı dizüstü bilgisayar", "Dizüstü Bilgisayar", 12999.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "TechMarka", new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(548), "Ergonomik kablosuz fare", "Kablosuz Fare", 299.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "ModaMarka", new DateTime(2025, 12, 7, 18, 31, 21, 310, DateTimeKind.Local).AddTicks(550), "Rahat pamuklu tişört", "Pamuklu Tişört", 199.99m });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsAdmin", "LastLoginAt", "Password", "Username" },
                values: new object[] { 1, new DateTime(2025, 12, 7, 18, 31, 21, 418, DateTimeKind.Local).AddTicks(8650), "admin@example.com", "Yönetici", true, null, "$2a$11$g/CXQbC8a/FS3feRRQJzAuUIMxvdvISzacjQm5QPw/vq147veig1W", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5414), "Electronic products", "Electronics" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5417), "Clothing and apparel", "Clothing" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5419), "Food and beverage products", "Food & Beverages" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5420), "Home and garden products", "Home & Garden" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "TechBrand", new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5527), "High-performance laptop", "Laptop Computer", 1299.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "TechBrand", new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5530), "Ergonomic wireless mouse", "Wireless Mouse", 29.99m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Brand", "CreatedAt", "Description", "Name", "Price" },
                values: new object[] { "FashionBrand", new DateTime(2025, 12, 7, 18, 11, 33, 322, DateTimeKind.Local).AddTicks(5532), "Comfortable cotton t-shirt", "Cotton T-Shirt", 19.99m });
        }
    }
}
