using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAcademyJWT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryToArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Artists");
        }
    }
}
