using System.Text.Json.Serialization;
using CodeCrewShop.Data;
using CodeCrewShop.Models;
using CodeCrewShop.Repositories.Implementations;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CodeCrewShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔗 Add DbContext with SQL Server
            builder.Services.AddDbContext<CodeCrewShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CodeCrewShopContext")
                    ?? throw new InvalidOperationException("Connection string 'CodeCrewShopContext' not found.")));
           
            
            //Add Cors Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyCores", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            // 🧩 Add Controllers

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            builder.Services.AddControllers().AddJsonOptions(options =>
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // 🧰 Register Repositories and Unit of Work
            builder.Services.AddScoped(typeof(IProductRepository<>), typeof(ProductRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            var app = builder.Build();
            app.UseCors("MyCores");

            // 🌐 Enable Swagger middleware in all environments (or only Development)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeCrewShop API v1");
                c.RoutePrefix = "swagger"; // so you can access at /swagger
            });

            // 🛡️ Enable HTTPS and Authorization
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // 🧭 Map Controllers
            app.MapControllers();

            app.Run();
        }
    }
}
