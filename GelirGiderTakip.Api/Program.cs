using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GelirGiderTakipDb")); 
});

builder.Services.AddScoped<IPasswordHasher<Kullanici>, PasswordHasher<Kullanici>>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services->Uygulamada kullanacaðýmýz servisleri ekliyoruz.
builder.Services.AddOpenApi();

//app->HTTP isteklerinin nasýl iþleneceðini ayarlýyoruz.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Gelir Gider Takip API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Controller'lardaki endpoint'leri HTTP isteklerine baðlar.
app.MapControllers();

app.Run();
