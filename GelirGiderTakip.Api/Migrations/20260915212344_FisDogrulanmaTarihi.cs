using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelirGiderTakip.Api.Migrations
{
    /// <inheritdoc />
    public partial class FisDogrulanmaTarihi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DogrulanmaTarihi",
                table: "Fisler",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DogrulanmaTarihi",
                table: "Fisler");
        }
    }
}
