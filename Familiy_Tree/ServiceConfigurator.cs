using Data;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Familiy_Tree.Business;


using System.Text;
using Microsoft.Extensions.Options;

namespace Familiy_Tree
{
    public class ServiceConfigurator
    {
        public static void ConfigureDBOptions(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Family_Tree"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }
        public static void ConfigureRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<UnitOfWork>();
        }
        public static void ConfigureBusiness(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<PersonsBusiness>();

        }

        internal static void ConfigureCompression(ResponseCompressionOptions options)
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        }



        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Access-Control-Allow-Origin", "Access-Control-Allow-Methods"));

            app.UseHttpsRedirection();

        }
    }
}
