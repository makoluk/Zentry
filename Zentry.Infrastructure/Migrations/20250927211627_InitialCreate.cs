using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zentry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDone = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAtUtc", "Description", "Icon", "IsActive", "Name", "SortOrder", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "#FF6B6B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "İzlemek istediğiniz filmler", "film", true, "İzlenecek Filmler", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "#4ECDC4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Günlük yapılacak işler", "calendar-day", true, "Günlük", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "#45B7D1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bir ara yapılacak işler", "clock", true, "Bir Ara", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "#96CEB4", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alınacak ürünler", "shopping-cart", true, "Alışveriş", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "#FFEAA7", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Okunacak kitaplar ve makaleler", "book", true, "Okuma", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedAtUtc",
                table: "Categories",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SortOrder",
                table: "Categories",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UpdatedAtUtc",
                table: "Categories",
                column: "UpdatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_IsDone",
                table: "Tasks",
                column: "IsDone");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UpdatedAtUtc",
                table: "Tasks",
                column: "UpdatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
