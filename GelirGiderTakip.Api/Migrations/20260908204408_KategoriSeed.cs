using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GelirGiderTakip.Api.Migrations
{
    /// <inheritdoc />
    public partial class KategoriSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Kategoriler",
                columns: new[] { "Id", "Ad", "AktifMi", "GuncellenmeTarihi", "KullaniciId", "OlusturulmaTarihi", "Tur" },
                values: new object[,]
                {
                    { 1, "Maaş", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1 },
                    { 2, "Ek Gelir", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1 },
                    { 3, "Yatırım Geliri", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1 },
                    { 4, "Kira Geliri", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1 },
                    { 5, "Diğer Gelir", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1 },
                    { 6, "Market", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 7, "Yeme İçme", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 8, "Ulaşım", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 9, "Faturalar", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 10, "Kira", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 11, "Sağlık", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 12, "Eğitim", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 13, "Giyim", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 14, "Eğlence", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 15, "Abonelikler", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 },
                    { 16, "Diğer Gider", true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
