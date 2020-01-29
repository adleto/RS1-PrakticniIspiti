using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RS1_Ispit_asp.net_core.Migrations
{
    public partial class dodani : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ispit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AngazovanId = table.Column<int>(nullable: false),
                    Datum = table.Column<DateTime>(nullable: false),
                    Napomena = table.Column<string>(nullable: true),
                    Zakljucano = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ispit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ispit_Angazovan_AngazovanId",
                        column: x => x.AngazovanId,
                        principalTable: "Angazovan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentIspit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IspitId = table.Column<int>(nullable: false),
                    Ocjena = table.Column<int>(nullable: false),
                    PristupioIspitu = table.Column<bool>(nullable: false),
                    SlusaPredmetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentIspit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentIspit_Ispit_IspitId",
                        column: x => x.IspitId,
                        principalTable: "Ispit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentIspit_SlusaPredmet_SlusaPredmetId",
                        column: x => x.SlusaPredmetId,
                        principalTable: "SlusaPredmet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ispit_AngazovanId",
                table: "Ispit",
                column: "AngazovanId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentIspit_IspitId",
                table: "StudentIspit",
                column: "IspitId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentIspit_SlusaPredmetId",
                table: "StudentIspit",
                column: "SlusaPredmetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentIspit");

            migrationBuilder.DropTable(
                name: "Ispit");
        }
    }
}
