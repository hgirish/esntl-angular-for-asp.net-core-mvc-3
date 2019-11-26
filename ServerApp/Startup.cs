using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ServerApp.Models;

namespace ServerApp
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(System.Environment.GetCommandLineArgs()
                .Skip(1).ToArray());
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString =
                Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddDbContext<IdentityDataContext>(options =>
            options.UseSqlServer(Configuration["ConnectionStrings:Identity"]));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddNewtonsoftJson();

            services.AddRazorPages();

            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1",
            //        new Microsoft.OpenApi.Models.OpenApiInfo
            //        {
            //            Title = "SportsStore API",
            //            Version = "v1"
            //        });
            //});

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "SessionData";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = "SportsStore.Session";
                options.IdleTimeout = System.TimeSpan.FromHours(48);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });
        }

        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IServiceProvider services,
            IAntiforgery antiforgery,
            IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/blazor",
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "../BlazorApp/wwwroot"))
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "",
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "./wwwroot/app"))
            });
            app.UseSession();         

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(nextDelegate => context =>
            {
                string path = context.Request.Path.Value;
                string[] directUrls = { "/admin", "/store", "/cart", "checkout" };
                if (path.StartsWith("/api") || 
                string .Equals("/", path) || 
                directUrls.Any(url=> path.StartsWith(url)))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN",
                        tokens.RequestToken, new Microsoft.AspNetCore.Http.CookieOptions()
                        {
                            HttpOnly = false,
                            Secure = false,
                            IsEssential = true
                        });
                }
                return nextDelegate(context);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "angular_fallback",
                    pattern: "{target:regex(admin|store|cart|checkout):nonfile}/{*catchall}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name:"blazor_integration",
                    pattern:"/blazor/{*path:nonfile}",
                    defaults: new {controller="Home", action="Blazor"}
                    );

                endpoints.MapRazorPages();
            });

            //app.Map("/blazor", opts =>
            //opts.UseClientSideBlazorFiles<BlazorApp.Startup>());
            app.UseClientSideBlazorFiles<BlazorApp.Startup>();


            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsStore API");
            //});


            //app.UseSpa(spa =>
            //{
            //    string strategy = Configuration.GetValue<string>("DevTools:ConnectionStrategy");
            //    if (strategy == "proxy")
            //    {
            //        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4200");
            //    }
            //    else if (strategy == "managed")
            //    {
            //        spa.Options.SourcePath = "../ClientApp";
            //        spa.UseAngularCliServer("start");
            //    }
                
            //});

            // SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
            // IdentitySeedData.SeedDatabase(services).Wait();

            if ((Configuration["INITDB"] ?? "false") == "true")
            {
                Console.WriteLine("Preparing Database...");
                SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
                IdentitySeedData.SeedDatabase(services).Wait();
                Console.WriteLine("Database preparation complete");
                lifetime.StopApplication();
            }
        }
    }
}
