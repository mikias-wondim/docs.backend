using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CreateFaqsInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Faqs",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Question = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Answer = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Order = table.Column<decimal>(type: "decimal(16,6)", precision: 16, scale: 6, nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeletedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                RecordStatus = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Faqs", x => x.Id);
                table.ForeignKey(
                    name: "FK_Faqs_Pages_PageId",
                    column: x => x.PageId,
                    principalSchema: "dbo",
                    principalTable: "Pages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Faqs_PageId",
            schema: "dbo",
            table: "Faqs",
            column: "PageId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Faqs",
            schema: "dbo");
    }
}
