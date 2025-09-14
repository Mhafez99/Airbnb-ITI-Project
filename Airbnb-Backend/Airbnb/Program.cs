
using Airbnb.Db;
using Airbnb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Airbnb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register DbContext
            builder.Services.AddDbContext<BookingContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Auth Service
            builder.Services.AddScoped<AuthService>();

            // Order Service
            builder.Services.AddScoped<OrderService>();

            // Stay Service
            builder.Services.AddScoped<StayService>();

            // Controllers
            builder.Services.AddControllers();


            builder.Services.AddOpenApi();
            // Swagger
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication("Bearer")
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["Jwt:Issuer"],
                       ValidAudience = builder.Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
                   };
               });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(option => option.SwaggerEndpoint("/openapi/v1.json", "V1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
