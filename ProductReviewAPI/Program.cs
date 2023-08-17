using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ProductReviewAPI.Extensions;
using ProductReviewAPI.Models;
using ProductReviewAPI.Repositories;

namespace ProductReviewAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            // Logger.
            builder.Logging.AddDebug();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Host.ConfigureLogging((logging) =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
                config.AddDebug();
            }).CreateLogger<Program>();

            // Add services to the container.
            AddJWTTokenServicesExtensions.AddJWTTokenServices(builder.Services, builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddCors();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });

            // Extract the connection string from config file.
            var connectionString = builder.Configuration.GetConnectionString("ReviewDBContext");

            // Inject the DbContext.
            builder.Services.AddDbContext<ReviewDBContext>(options =>
                options.UseSqlServer(connectionString)
            );

            builder.Services.AddScoped<IDataRepository<User>, UserRepository>();
            builder.Services.AddScoped<IDataRepository<Product>, ProductRepository>();
            builder.Services.AddScoped<IDataRepository<Category>, CategoryRepository>();
            builder.Services.AddScoped<IDataRepository<Seller>, SellerRepository>();
            builder.Services.AddScoped<IDataRepository<Review>, ReviewRepository>();
            builder.Services.AddScoped<IDataRepository<ProductImage>, ProductImageRepository>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}