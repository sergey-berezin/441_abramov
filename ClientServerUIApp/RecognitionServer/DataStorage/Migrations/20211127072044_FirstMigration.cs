using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataStorage.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesInfo",
                columns: table => new
                {
                    ImageInfoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageName = table.Column<string>(type: "TEXT", nullable: true),
                    ImageHash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesInfo", x => x.ImageInfoId);
                });

            migrationBuilder.CreateTable(
                name: "ImagesInfoDetails",
                columns: table => new
                {
                    ImageInfoDetailsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageInfoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesInfoDetails", x => x.ImageInfoDetailsId);
                    table.ForeignKey(
                        name: "FK_ImagesInfoDetails_ImagesInfo_ImageInfoId",
                        column: x => x.ImageInfoId,
                        principalTable: "ImagesInfo",
                        principalColumn: "ImageInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecognizedObjects",
                columns: table => new
                {
                    ObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageInfoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Confidence = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognizedObjects", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_RecognizedObjects_ImagesInfo_ImageInfoId",
                        column: x => x.ImageInfoId,
                        principalTable: "ImagesInfo",
                        principalColumn: "ImageInfoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesInfoDetails_ImageInfoId",
                table: "ImagesInfoDetails",
                column: "ImageInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecognizedObjects_ImageInfoId",
                table: "RecognizedObjects",
                column: "ImageInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesInfoDetails");

            migrationBuilder.DropTable(
                name: "RecognizedObjects");

            migrationBuilder.DropTable(
                name: "ImagesInfo");
        }
    }
}
