using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Selectio.Data.Migrations
{
    public partial class decsriptionForTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SolvingOutput",
                table: "SqlSolvings");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SqlTasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SqlTasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SolvedAt",
                table: "SqlSolvings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SqlTasks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SqlTasks");

            migrationBuilder.DropColumn(
                name: "SolvedAt",
                table: "SqlSolvings");

            migrationBuilder.AddColumn<string>(
                name: "SolvingOutput",
                table: "SqlSolvings",
                nullable: false,
                defaultValue: "");
        }
    }
}
