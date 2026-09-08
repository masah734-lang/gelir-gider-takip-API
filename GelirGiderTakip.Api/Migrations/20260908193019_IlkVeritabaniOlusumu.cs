using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelirGiderTakip.Api.Migrations
{
    /// <inheritdoc />
    public partial class IlkVeritabaniOlusumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ParolaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fisler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DosyaYolu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrijinalDosyaAdi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HamOcrMetni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlgilananIsletmeAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AlgilananTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AlgilananTarih = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<byte>(type: "tinyint", nullable: false),
                    HataMesaji = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    YuklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fisler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Isletmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturanKullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Isletmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Isletmeler_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tur = table.Column<byte>(type: "tinyint", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategoriler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Butceler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Yil = table.Column<short>(type: "smallint", nullable: false),
                    Ay = table.Column<byte>(type: "tinyint", nullable: false),
                    LimitTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Butceler", x => x.Id);
                    table.CheckConstraint("CK_Butceler_Ay", "[Ay] BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_Butceler_LimitTutari", "[LimitTutari] > 0");
                    table.CheckConstraint("CK_Butceler_Yil", "[Yil] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_Butceler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Butceler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinansalIslemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<byte>(type: "tinyint", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: true),
                    FisId = table.Column<int>(type: "int", nullable: true),
                    IsletmeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinansalIslemler", x => x.Id);
                    table.CheckConstraint("CK_FinansalIslemler_Tur", "[Tur] IN (1, 2)");
                    table.CheckConstraint("CK_FinansalIslemler_Tutar", "[Tutar] > 0");
                    table.ForeignKey(
                        name: "FK_FinansalIslemler_Fisler_FisId",
                        column: x => x.FisId,
                        principalTable: "Fisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinansalIslemler_Isletmeler_IsletmeId",
                        column: x => x.IsletmeId,
                        principalTable: "Isletmeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinansalIslemler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinansalIslemler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FisDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FisId = table.Column<int>(type: "int", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: true),
                    UrunAdi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HamSatirMetni = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SiraNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FisDetaylari_Fisler_FisId",
                        column: x => x.FisId,
                        principalTable: "Fisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FisDetaylari_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<byte>(type: "tinyint", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OkunmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    ButceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.CheckConstraint("CK_Bildirimler_Tur", "[Tur] IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_Bildirimler_Butceler_ButceId",
                        column: x => x.ButceId,
                        principalTable: "Butceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bildirimler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_ButceId",
                table: "Bildirimler",
                column: "ButceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId_OkunmaTarihi_OlusturulmaTarihi",
                table: "Bildirimler",
                columns: new[] { "KullaniciId", "OkunmaTarihi", "OlusturulmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Butceler_KategoriId",
                table: "Butceler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Butceler_KullaniciId_KategoriId_Yil_Ay",
                table: "Butceler",
                columns: new[] { "KullaniciId", "KategoriId", "Yil", "Ay" },
                unique: true,
                filter: "[KategoriId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Butceler_KullaniciId_Yil_Ay",
                table: "Butceler",
                columns: new[] { "KullaniciId", "Yil", "Ay" },
                unique: true,
                filter: "[KategoriId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinansalIslemler_FisId",
                table: "FinansalIslemler",
                column: "FisId",
                unique: true,
                filter: "[FisId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinansalIslemler_IsletmeId",
                table: "FinansalIslemler",
                column: "IsletmeId");

            migrationBuilder.CreateIndex(
                name: "IX_FinansalIslemler_KategoriId",
                table: "FinansalIslemler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_FinansalIslemler_KullaniciId_IslemTarihi",
                table: "FinansalIslemler",
                columns: new[] { "KullaniciId", "IslemTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_FisDetaylari_FisId_SiraNo",
                table: "FisDetaylari",
                columns: new[] { "FisId", "SiraNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FisDetaylari_KategoriId",
                table: "FisDetaylari",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Fisler_KullaniciId_YuklenmeTarihi",
                table: "Fisler",
                columns: new[] { "KullaniciId", "YuklenmeTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Isletmeler_Ad",
                table: "Isletmeler",
                column: "Ad",
                unique: true,
                filter: "[OlusturanKullaniciId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Isletmeler_OlusturanKullaniciId_Ad",
                table: "Isletmeler",
                columns: new[] { "OlusturanKullaniciId", "Ad" },
                unique: true,
                filter: "[OlusturanKullaniciId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_Ad_Tur",
                table: "Kategoriler",
                columns: new[] { "Ad", "Tur" },
                unique: true,
                filter: "[KullaniciId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_KullaniciId_Ad_Tur",
                table: "Kategoriler",
                columns: new[] { "KullaniciId", "Ad", "Tur" },
                unique: true,
                filter: "[KullaniciId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Eposta",
                table: "Kullanicilar",
                column: "Eposta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "FinansalIslemler");

            migrationBuilder.DropTable(
                name: "FisDetaylari");

            migrationBuilder.DropTable(
                name: "Butceler");

            migrationBuilder.DropTable(
                name: "Isletmeler");

            migrationBuilder.DropTable(
                name: "Fisler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
