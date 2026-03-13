using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveAuction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSearchCountToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "SearchCount",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchCount",
                table: "Categories");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AspNetUsers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
