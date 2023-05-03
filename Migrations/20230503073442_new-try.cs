using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class newtry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adverstisements_AspNetUsers_AdPostedById",
                table: "adverstisements");

            migrationBuilder.DropIndex(
                name: "IX_adverstisements_AdPostedById",
                table: "adverstisements");

            migrationBuilder.AlterColumn<string>(
                name: "AdPostedById",
                table: "adverstisements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdPostedById",
                table: "adverstisements",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_adverstisements_AdPostedById",
                table: "adverstisements",
                column: "AdPostedById");

            migrationBuilder.AddForeignKey(
                name: "FK_adverstisements_AspNetUsers_AdPostedById",
                table: "adverstisements",
                column: "AdPostedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
