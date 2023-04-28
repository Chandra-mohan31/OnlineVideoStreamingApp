using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class tablelikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "likesTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    LikedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_likesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_likesTable_AspNetUsers_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_likesTable_videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_likesTable_LikedUserId",
                table: "likesTable",
                column: "LikedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_likesTable_VideoId",
                table: "likesTable",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "likesTable");
        }
    }
}
