using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveAuction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditedRowVersionInAuctionTouint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Auctions",
                newName: "xmin");

            migrationBuilder.AlterColumn<uint>(
                name: "xmin",
                table: "Auctions",
                type: "xid",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "xmin",
                table: "Auctions",
                newName: "RowVersion");

            migrationBuilder.AlterColumn<string>(
                name: "RowVersion",
                table: "Auctions",
                type: "text",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true);
        }
    }
}
