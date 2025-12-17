using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace talearc_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationKeyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registration_keys",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_keys", x => x.key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_keys");
        }
    }
}
