using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CreatePagesInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Pages",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ParentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ContentMd = table.Column<string>(type: "nvarchar(max)", maxLength: 100000, nullable: true),
                Order = table.Column<decimal>(type: "decimal(16,6)", precision: 16, scale: 6, nullable: false),
                Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                table.PrimaryKey("PK_Pages", x => x.Id);
                table.ForeignKey(
                    name: "FK_Pages_Pages_ParentPageId",
                    column: x => x.ParentPageId,
                    principalSchema: "dbo",
                    principalTable: "Pages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Pages_Sections_SectionId",
                    column: x => x.SectionId,
                    principalSchema: "dbo",
                    principalTable: "Sections",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Pages_ParentPageId",
            schema: "dbo",
            table: "Pages",
            column: "ParentPageId");

        migrationBuilder.CreateIndex(
            name: "IX_Pages_SectionId",
            schema: "dbo",
            table: "Pages",
            column: "SectionId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Pages",
            schema: "dbo");
    }
}
