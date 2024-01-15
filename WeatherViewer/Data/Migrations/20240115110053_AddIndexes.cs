using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherViewer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_locations_user_id",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_users_login",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_locations_user_id_location_id",
                table: "locations",
                columns: new[] { "user_id", "location_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_login",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_locations_user_id_location_id",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_locations_user_id",
                table: "locations",
                column: "user_id");
        }
    }
}
