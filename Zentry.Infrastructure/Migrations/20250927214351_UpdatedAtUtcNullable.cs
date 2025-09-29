using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zentry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAtUtcNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Tasks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime('now')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Categories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "datetime('now')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Tasks",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
