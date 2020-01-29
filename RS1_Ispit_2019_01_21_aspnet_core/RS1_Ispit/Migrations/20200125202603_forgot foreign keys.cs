using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RS1_Ispit_asp.net_core.Migrations
{
    public partial class forgotforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IspitStavka_MaturskiIspit_MaturskiIspitId",
                table: "IspitStavka");

            migrationBuilder.DropForeignKey(
                name: "FK_IspitStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                table: "IspitStavka");

            migrationBuilder.AlterColumn<int>(
                name: "OdjeljenjeStavkaId",
                table: "IspitStavka",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaturskiIspitId",
                table: "IspitStavka",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IspitStavka_MaturskiIspit_MaturskiIspitId",
                table: "IspitStavka",
                column: "MaturskiIspitId",
                principalTable: "MaturskiIspit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IspitStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                table: "IspitStavka",
                column: "OdjeljenjeStavkaId",
                principalTable: "OdjeljenjeStavka",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IspitStavka_MaturskiIspit_MaturskiIspitId",
                table: "IspitStavka");

            migrationBuilder.DropForeignKey(
                name: "FK_IspitStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                table: "IspitStavka");

            migrationBuilder.AlterColumn<int>(
                name: "OdjeljenjeStavkaId",
                table: "IspitStavka",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "MaturskiIspitId",
                table: "IspitStavka",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_IspitStavka_MaturskiIspit_MaturskiIspitId",
                table: "IspitStavka",
                column: "MaturskiIspitId",
                principalTable: "MaturskiIspit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IspitStavka_OdjeljenjeStavka_OdjeljenjeStavkaId",
                table: "IspitStavka",
                column: "OdjeljenjeStavkaId",
                principalTable: "OdjeljenjeStavka",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
