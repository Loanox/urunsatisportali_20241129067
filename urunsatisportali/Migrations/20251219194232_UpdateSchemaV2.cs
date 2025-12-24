using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urunsatisportali.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4090));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4092));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4093));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4095));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4311));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4314));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 19, 21, 21, 44, 18, DateTimeKind.Local).AddTicks(4316));
        }
    }
}
