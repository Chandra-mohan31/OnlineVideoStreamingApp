using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVideoStreamingApp.Migrations
{
    /// <inheritdoc />
    public partial class addeddatetimecomments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommentedOn",
                table: "commentsTable",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentedOn",
                table: "commentsTable");
        }
    }
}
