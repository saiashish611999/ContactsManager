using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_CountryId",
                schema: "ContactsManager",
                table: "Persons");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryId",
                schema: "ContactsManager",
                table: "Persons",
                column: "CountryId",
                principalSchema: "ContactsManager",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_CountryId",
                schema: "ContactsManager",
                table: "Persons");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryId",
                schema: "ContactsManager",
                table: "Persons",
                column: "CountryId",
                principalSchema: "ContactsManager",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
