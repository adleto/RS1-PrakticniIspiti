using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RS1_Ispit_asp.net_core.Migrations
{
    public partial class entiteti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Datum = table.Column<DateTime>(nullable: false),
                    PredajePredmetId = table.Column<int>(nullable: false),
                    Sadrzaj = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cas_PredajePredmet_PredajePredmetId",
                        column: x => x.PredajePredmetId,
                        principalTable: "PredajePredmet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CasStavka",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CasId = table.Column<int>(nullable: false),
                    Napomena = table.Column<string>(nullable: true),
                    Ocjena = table.Column<int>(nullable: false),
                    OdjeljenjeStavkaId = table.Column<int>(nullable: false),
                    OpravdanoOdsutan = table.Column<bool>(nullable: false),
                    Prisutan = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasStavka", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasStavka_Cas_CasId",
                        column: x => x.CasId,
                        principalTable: "Cas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                        column: x => x.OdjeljenjeStavkaId,
                        principalTable: "OdjeljenjeStavka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cas_PredajePredmetId",
                table: "Cas",
                column: "PredajePredmetId");

            migrationBuilder.CreateIndex(
                name: "IX_CasStavka_CasId",
                table: "CasStavka",
                column: "CasId");

            migrationBuilder.CreateIndex(
                name: "IX_CasStavka_OdjeljenjeStavkaId",
                table: "CasStavka",
                column: "OdjeljenjeStavkaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasStavka");

            migrationBuilder.DropTable(
                name: "Cas");
        }
    }
}
