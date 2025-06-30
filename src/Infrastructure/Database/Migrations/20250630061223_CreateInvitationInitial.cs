using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class CreateInvitationInitial : Migration
{
    private static readonly string[] columns = ["ProjectId", "InvitedUserId", "InvitedByUserId"];
    private static readonly string[] columnsArray = ["ProjectId", "InvitedUserId", "Status"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Invitations",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvitedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvitedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Role = table.Column<int>(type: "int", nullable: false),
                SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_Invitations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Invitations_Projects_ProjectId",
                    column: x => x.ProjectId,
                    principalSchema: "dbo",
                    principalTable: "Projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Invitations_Users_InvitedByUserId",
                    column: x => x.InvitedByUserId,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Invitations_Users_InvitedUserId",
                    column: x => x.InvitedUserId,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Invitations_InvitedByUserId",
            schema: "dbo",
            table: "Invitations",
            column: "InvitedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Invitations_InvitedUserId",
            schema: "dbo",
            table: "Invitations",
            column: "InvitedUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Invitations_ProjectId_InvitedUserId_InvitedByUserId",
            schema: "dbo",
            table: "Invitations",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Invitations_ProjectId_InvitedUserId_Status",
            schema: "dbo",
            table: "Invitations",
            columns: columnsArray,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Invitations",
            schema: "dbo");
    }
}
