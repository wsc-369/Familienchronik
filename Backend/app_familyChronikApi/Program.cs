
using app_familyBackend.DataContext;
using app_familyBackend.MigrationOfData;
using app_familyBackend.PdfExtractor;
using app_familyChronikApi.ReadWriteDB;
using app_familyBackend.Services;
using app_familyChronikApi.Services;
using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungData.Models.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace app_familyChronikApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddOpenApi(); // .NET 9 built-in

      builder.Services.AddDbContext<MyDatabaseContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChronikDateConnection")));

      builder.Services.AddDbContext<wsc_chronikContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChronikDateConnection")));

      builder.Services.AddControllers(o => { o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; });

      // --- builder.Services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });

      // // 🔑 Eigene Services registrieren
      builder.Services.AddScoped<ReadPersonRelations>();
      builder.Services.AddScoped<ReadWiteDialects>();
      builder.Services.AddScoped<DoMigratonData>();
      builder.Services.AddScoped<ReadWirtePersons>();
      builder.Services.AddScoped<ReadWirteContents>();
      builder.Services.AddScoped<PdfTextExtractor>();
      builder.Services.AddScoped<PdfProcessingService>();
      builder.Services.AddScoped<SearchService>();
      

      var config = builder.Configuration;


      var corsEndpoints = config.GetSection("Cors:Endpoints").Get<string[]>();
      builder.Services.AddCors(options =>
      {
        options.AddPolicy("MyApplication", builder =>
        {
          builder.WithOrigins(origins: corsEndpoints)
          .AllowAnyHeader()
          .AllowAnyMethod()
          //.AllowCredentials()
          ;
        });
      });

      // Add services to the container.

      builder.Services.AddControllers();
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();

      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = ReadSettings.UrlTickenValidation(),
            ValidAudience = ReadSettings.UrlTickenValidation(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345_1111111111111"))
          };
        });



      var app = builder.Build();

      app.MapOpenApi();

      app.MapScalarApiReference(options => { 
        options.WithTitle("My API"); 
      });

      app.UseHttpsRedirection();
      app.UseStaticFiles(new StaticFileOptions()
      {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"resources")),
        RequestPath = new PathString("/resources")
      });
      //var resourcesPath = Path.Combine(builder.Environment.ContentRootPath, "resources"); 
      //app.UseStaticFiles(new StaticFileOptions { 
      //  FileProvider = new PhysicalFileProvider(resourcesPath),
      //  RequestPath = "/resources" });

      app.UseRouting();
      app.UseCors("MyApplication");
      app.UseAuthentication();
      app.UseAuthorization();
      app.MapControllers();

      app.Run();
    }
  }
}
