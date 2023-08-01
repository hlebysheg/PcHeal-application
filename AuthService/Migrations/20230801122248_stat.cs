using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    public partial class stat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PcStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PcName = table.Column<string>(type: "TEXT", nullable: false),
                    CPUName = table.Column<string>(type: "TEXT", nullable: false),
                    CPUTemp = table.Column<float>(type: "REAL", nullable: true),
                    CPULoad = table.Column<float>(type: "REAL", nullable: false),
                    CPUFrenq = table.Column<float>(type: "REAL", nullable: true),
                    GPUName = table.Column<string>(type: "TEXT", nullable: false),
                    GPUTemp = table.Column<float>(type: "REAL", nullable: true),
                    GPULoad = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcStatistic_Student_UserId",
                        column: x => x.UserId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Email", "Name", "Password", "RoleId" },
                values: new object[] { 500, "string", "string", "string", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_PcStatistic_UserId",
                table: "PcStatistic",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcStatistic");

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 500);
        }
    }
}
