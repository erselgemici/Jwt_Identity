using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAcademyJWT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeezerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "Songs");

            migrationBuilder.AddColumn<long>(
                name: "DeezerTrackId",
                table: "Songs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeezerTrackId",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
