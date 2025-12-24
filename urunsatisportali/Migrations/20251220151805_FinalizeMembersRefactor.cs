using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urunsatisportali.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeMembersRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(4835));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(4836));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(4838));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(4879));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(5044));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(5046));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 18, 4, 765, DateTimeKind.Local).AddTicks(5049));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3423));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3425));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3427));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3428));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3561));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3564));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 20, 18, 16, 5, 767, DateTimeKind.Local).AddTicks(3566));
        }
    }
}
