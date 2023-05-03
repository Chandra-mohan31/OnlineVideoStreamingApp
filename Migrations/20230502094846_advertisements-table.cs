using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class advertisementstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adverstisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdvertisementDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdPosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdPostedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adverstisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adverstisements_AspNetUsers_AdPostedById",
                        column: x => x.AdPostedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_adverstisements_AdPostedById",
                table: "adverstisements",
                column: "AdPostedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adverstisements");
        }
    }
}
