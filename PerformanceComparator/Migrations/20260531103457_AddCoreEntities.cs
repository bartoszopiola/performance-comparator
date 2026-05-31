using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceComparator.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Funds_Ticker",
                table: "Funds");

            migrationBuilder.DropIndex(
                name: "IX_AssetClasses_Slug",
                table: "AssetClasses");

            migrationBuilder.DropColumn(
                name: "Nav",
                table: "NavRecords");

            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "Ticker",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "AssetClasses");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Funds",
                newName: "IsBenchmark");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ContentBlocks",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "NavRecords",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Funds",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LogoFileName",
                table: "Funds",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "Funds",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Funds",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "ContentBlocks",
                type: "TEXT",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ContentBlocks",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "NavRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "LogoFileName",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ContentBlocks");

            migrationBuilder.RenameColumn(
                name: "IsBenchmark",
                table: "Funds",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ContentBlocks",
                newName: "Content");

            migrationBuilder.AddColumn<decimal>(
                name: "Nav",
                table: "NavRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "Funds",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ticker",
                table: "Funds",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "AssetClasses",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Ticker",
                table: "Funds",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetClasses_Slug",
                table: "AssetClasses",
                column: "Slug",
                unique: true);
        }
    }
}
