using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations;

/// <inheritdoc />
public partial class AddVerificationCodeTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NickName",
            table: "Users");

        migrationBuilder.RenameColumn(
            name: "Password",
            table: "Users",
            newName: "Phone");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreateDate",
            table: "Users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "VerificationCodes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "text", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_VerificationCodes_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_VerificationCodes_UserId",
            table: "VerificationCodes",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "VerificationCodes");

        migrationBuilder.DropColumn(
            name: "CreateDate",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Users");

        migrationBuilder.RenameColumn(
            name: "Phone",
            table: "Users",
            newName: "Password");

        migrationBuilder.AddColumn<string>(
            name: "NickName",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
