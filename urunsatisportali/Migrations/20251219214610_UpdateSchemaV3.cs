using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urunsatisportali.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(772));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(775));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(777));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(779));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(945));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(948));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 0, 46, 9, 779, DateTimeKind.Local).AddTicks(950));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(7834));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(7838));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(7840));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(7842));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(8248));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(8255));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 22, 42, 32, 325, DateTimeKind.Local).AddTicks(8259));
        }
    }
}
