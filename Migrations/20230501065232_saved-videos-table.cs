using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class savedvideostable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "savedVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SavedVideoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_savedVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_savedVideos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_savedVideos_videos_SavedVideoId",
                        column: x => x.SavedVideoId,
                        principalTable: "videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_savedVideos_SavedVideoId",
                table: "savedVideos",
                column: "SavedVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_savedVideos_UserId",
                table: "savedVideos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "savedVideos");
        }
    }
}
