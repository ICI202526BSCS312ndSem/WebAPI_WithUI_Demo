
using API.DAL;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var key = Encoding.ASCII.GetBytes("ANOBAMAKULITKABA_ANOBAMAKULITKABA");

            var builder = WebApplication.CreateBuilder(args);

            // Add Authentication Services and set the Default Scheme
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // new skill unlock ctrl + .
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanView", policy => policy.RequireClaim("permission", "view_product"));
                options.AddPolicy("CanAdd", policy => policy.RequireClaim("permission", "add_product"));
                options.AddPolicy("CanEdit", policy => policy.RequireClaim("permission", "edit_product"));
                options.AddPolicy("CanDelete", policy => policy.RequireClaim("permission", "delete_product"));
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    // Replace with the actual URL/Port your UI project is running on
                    policy.WithOrigins("http://localhost:7056", "https://localhost:7056")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddSingleton<ProductRepository>();
            builder.Services.AddScoped<ProductService>();
            
            // builder.Website.UseUrls(""https://localhost:7062); 
            // 5000 error

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
