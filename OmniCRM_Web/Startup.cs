using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;
using System;
using System.Text;

namespace OmniCRM_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //IdentityModelEventSource.ShowPII = true;

            #region Database connection
            services.AddDbContext<OmniCRMContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            #endregion

            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromHours(6);
                option.Cookie.IsEssential = true;
            });

            //services.AddControllersWithViews();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            #region Add jwt authentication
            var key = Encoding.ASCII.GetBytes(Configuration["TokenSettings:JWT_Secret"].ToString());
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,

                    ValidIssuer = Configuration["TokenSettings:Client_URL"].ToString(),
                    ValidAudience = Configuration["TokenSettings:Client_URL"].ToString(),

                    ClockSkew = TimeSpan.Zero,
                };
            });

            #endregion

            services.AddAuthorization();

            //Add Auto Mapper for model and View model
            services.AddAutoMapper(typeof(Startup));

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            DailyEmailService._configuration = Configuration;
            //DailyEmailService._hostingEnvironment = HostingEnvironment;
            GenericMethods._hostingEnvironment = HostingEnvironment;
            //DailyEmailService.StartTimer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseCookiePolicy();
            app.UseSession();

            app.UseRouting();
            app.UseCertificateForwarding();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.Options.StartupTimeout = new TimeSpan(0, 0, 80);
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
