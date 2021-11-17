using Microsoft.EntityFrameworkCore.Migrations;

namespace DataStorage.Migrations
{
    public partial class SecondVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagesInfo_ImagesInfoDetails_ImageInfoDetailsId",
                table: "ImagesInfo");

            migrationBuilder.DropIndex(
                name: "IX_ImagesInfo_ImageInfoDetailsId",
                table: "ImagesInfo");

            migrationBuilder.DropColumn(
                name: "ImageInfoDetailsId",
                table: "ImagesInfo");

            migrationBuilder.AddColumn<int>(
                name: "ImageInfoId",
                table: "ImagesInfoDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImagesInfoDetails_ImageInfoId",
                table: "ImagesInfoDetails",
                column: "ImageInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImagesInfoDetails_ImagesInfo_ImageInfoId",
                table: "ImagesInfoDetails",
                column: "ImageInfoId",
                principalTable: "ImagesInfo",
                principalColumn: "ImageInfoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagesInfoDetails_ImagesInfo_ImageInfoId",
                table: "ImagesInfoDetails");

            migrationBuilder.DropIndex(
                name: "IX_ImagesInfoDetails_ImageInfoId",
                table: "ImagesInfoDetails");

            migrationBuilder.DropColumn(
                name: "ImageInfoId",
                table: "ImagesInfoDetails");

            migrationBuilder.AddColumn<int>(
                name: "ImageInfoDetailsId",
                table: "ImagesInfo",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagesInfo_ImageInfoDetailsId",
                table: "ImagesInfo",
                column: "ImageInfoDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagesInfo_ImagesInfoDetails_ImageInfoDetailsId",
                table: "ImagesInfo",
                column: "ImageInfoDetailsId",
                principalTable: "ImagesInfoDetails",
                principalColumn: "ImageInfoDetailsId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
