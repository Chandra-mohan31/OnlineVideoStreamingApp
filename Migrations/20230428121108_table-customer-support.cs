using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class tablecustomersupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customerSupportTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueryPostedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerSupportTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customerSupportTable_AspNetUsers_QueryPostedUserId",
                        column: x => x.QueryPostedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_customerSupportTable_QueryPostedUserId",
                table: "customerSupportTable",
                column: "QueryPostedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customerSupportTable");
        }
    }
}
