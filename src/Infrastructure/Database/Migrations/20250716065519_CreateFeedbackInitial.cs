using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CreateFeedbackInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Feedbacks",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Comment = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                Rating = table.Column<int>(type: "int", nullable: false),
                IsRead = table.Column<bool>(type: "bit", nullable: false),
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
                table.PrimaryKey("PK_Feedbacks", x => x.Id);
                table.ForeignKey(
                    name: "FK_Feedbacks_Pages_PageId",
                    column: x => x.PageId,
                    principalSchema: "dbo",
                    principalTable: "Pages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Feedbacks_PageId",
            schema: "dbo",
            table: "Feedbacks",
            column: "PageId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Feedbacks",
            schema: "dbo");
    }
}
