using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Selectio.Data.Migrations
{
    public partial class initialMigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SqlSolvings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    Solving = table.Column<string>(nullable: false),
                    SolvingOutput = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlSolvings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SqlTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creates = table.Column<string>(nullable: false),
                    Inserts = table.Column<string>(nullable: false),
                    IsWriteAction = table.Column<bool>(nullable: false),
                    Solving = table.Column<string>(nullable: false),
                    SolvingOutput = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlTasks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SqlSolvings");

            migrationBuilder.DropTable(
                name: "SqlTasks");
        }
    }
}
