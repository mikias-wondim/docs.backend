using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CreateSectionInitial : Migration
{
    private static readonly string[] columns = new[] { "SectionId", "UserId" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TodoItems",
            schema: "dbo");

        migrationBuilder.CreateTable(
            name: "Sections",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Visibility = table.Column<int>(type: "int", nullable: false),
                Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                AllowedRoles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_Sections", x => x.Id);
                table.ForeignKey(
                    name: "FK_Sections_Projects_ProjectId",
                    column: x => x.ProjectId,
                    principalSchema: "dbo",
                    principalTable: "Projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SectionUserAccesses",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                table.PrimaryKey("PK_SectionUserAccesses", x => x.Id);
                table.ForeignKey(
                    name: "FK_SectionUserAccesses_Sections_SectionId",
                    column: x => x.SectionId,
                    principalSchema: "dbo",
                    principalTable: "Sections",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SectionUserAccesses_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Sections_ProjectId",
            schema: "dbo",
            table: "Sections",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_SectionUserAccesses_SectionId_UserId",
            schema: "dbo",
            table: "SectionUserAccesses",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_SectionUserAccesses_UserId",
            schema: "dbo",
            table: "SectionUserAccesses",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SectionUserAccesses",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "Sections",
            schema: "dbo");

        migrationBuilder.CreateTable(
            name: "TodoItems",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                Labels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                RecordStatus = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TodoItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_TodoItems_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TodoItems_UserId",
            schema: "dbo",
            table: "TodoItems",
            column: "UserId");
    }
}
