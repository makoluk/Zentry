using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zentry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHabits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TargetValue = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HabitEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HabitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitEntries_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Habits",
                columns: new[] { "Id", "Color", "CreatedAtUtc", "Description", "Icon", "IsActive", "Name", "SortOrder", "TargetValue", "Type", "Unit", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), "#3B82F6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günde 8 bardak su içme hedefi", "droplets", true, "Su İçmek", 1, 8, 1, "bardak", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), "#EF4444", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günlük spor yapma", "activity", true, "Egzersiz", 2, null, 0, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), "#10B981", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günlük sayfa okuma hedefi", "book", true, "Kitap Okuma", 3, 20, 1, "sayfa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a4444444-4444-4444-4444-444444444444"), "#8B5CF6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günlük meditasyon pratiği", "brain", true, "Meditasyon", 4, null, 0, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitEntries_Date",
                table: "HabitEntries",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_HabitEntries_HabitId",
                table: "HabitEntries",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitEntries_HabitId_Date",
                table: "HabitEntries",
                columns: new[] { "HabitId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_IsActive",
                table: "Habits",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_IsActive_SortOrder",
                table: "Habits",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_SortOrder",
                table: "Habits",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitEntries");

            migrationBuilder.DropTable(
                name: "Habits");
        }
    }
}
