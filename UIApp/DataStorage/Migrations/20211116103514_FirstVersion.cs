using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataStorage.Migrations
{
    public partial class FirstVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesInfoDetails",
                columns: table => new
                {
                    ImageInfoDetailsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesInfoDetails", x => x.ImageInfoDetailsId);
                });

            migrationBuilder.CreateTable(
                name: "ImagesInfo",
                columns: table => new
                {
                    ImageInfoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageName = table.Column<string>(type: "TEXT", nullable: true),
                    ImageHash = table.Column<string>(type: "TEXT", nullable: true),
                    ImageInfoDetailsId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesInfo", x => x.ImageInfoId);
                    table.ForeignKey(
                        name: "FK_ImagesInfo_ImagesInfoDetails_ImageInfoDetailsId",
                        column: x => x.ImageInfoDetailsId,
                        principalTable: "ImagesInfoDetails",
                        principalColumn: "ImageInfoDetailsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecognizedObjects",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Confidence = table.Column<double>(type: "REAL", nullable: false),
                    ImageInfoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognizedObjects", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_RecognizedObjects_ImagesInfo_ImageInfoId",
                        column: x => x.ImageInfoId,
                        principalTable: "ImagesInfo",
                        principalColumn: "ImageInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesInfo_ImageInfoDetailsId",
                table: "ImagesInfo",
                column: "ImageInfoDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_RecognizedObjects_ImageInfoId",
                table: "RecognizedObjects",
                column: "ImageInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecognizedObjects");

            migrationBuilder.DropTable(
                name: "ImagesInfo");

            migrationBuilder.DropTable(
                name: "ImagesInfoDetails");
        }
    }
}
