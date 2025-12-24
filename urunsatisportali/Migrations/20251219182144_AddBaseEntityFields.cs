using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urunsatisportali.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SaleItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SaleItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SaleItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsActive", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4090), true, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsActive", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4092), true, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsActive", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4093), true, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "IsActive", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4095), true, false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4311), false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4314), false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsDeleted" },
                values: new object[] { new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4316), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4550));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4552));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4554));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4556));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4707));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4711));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 17, 41, 14, 98, DateTimeKind.Local).AddTicks(4713));
        }
    }
}
