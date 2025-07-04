using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateSectionDefaultPage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "DefaultPageId",
            schema: "dbo",
            table: "Sections",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.CreateIndex(
            name: "IX_Sections_DefaultPageId",
            schema: "dbo",
            table: "Sections",
            column: "DefaultPageId");

        migrationBuilder.AddForeignKey(
            name: "FK_Sections_Pages_DefaultPageId",
            schema: "dbo",
            table: "Sections",
            column: "DefaultPageId",
            principalSchema: "dbo",
            principalTable: "Pages",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Sections_Pages_DefaultPageId",
            schema: "dbo",
            table: "Sections");

        migrationBuilder.DropIndex(
            name: "IX_Sections_DefaultPageId",
            schema: "dbo",
            table: "Sections");

        migrationBuilder.DropColumn(
            name: "DefaultPageId",
            schema: "dbo",
            table: "Sections");
    }
}
