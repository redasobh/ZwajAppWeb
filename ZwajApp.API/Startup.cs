using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using ZwajApp.API.Data;
using ZwajApp.API.Helpers;
using ZwajApp.API.Models;

namespace ZwajApp.API {
    public class Startup {
        
        public Startup (IConfiguration configuration) {
            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<DataContext>(x => x.
            UseMySql (Configuration.GetConnectionString ("DefaultConnection")).
            ConfigureWarnings(warnings=> warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));
          // var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Secret phase"));
            IdentityBuilder builder = services.AddIdentityCore<User>(opt=>{
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            //Authentication MiddleWare
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options => {
                    Options.TokenValidationParameters=new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes (Configuration.GetSection ("AppSettings:Token").Value)),
                    ValidateIssuer =false,
                    ValidateAudience =false
                    };
                });
                services.AddAuthorization(options=>{
                    options.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
                    options.AddPolicy("ModeratePhotoRole", policy=>policy.RequireRole("Admin","Moderator"));
                    options.AddPolicy("VipOnly", policy=>policy.RequireRole("VIP"));
                }
                );
            services.AddMvc(options=> {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(option => {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });            
            services.AddSingleton(typeof(IConverter),new SynchronizedConverter(new PdfTools()));
            services.AddCors();
            services.AddSignalR();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddAutoMapper();
            //Mapper.Reset();
            services.AddTransient<TrailData>();
           // services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IZwajRepository, ZwajRepository>();
            services.AddScoped<LogUserActivity>();            
        }
        public void ConfigureDevelopmentServices (IServiceCollection services) {
            services.AddDbContext<DataContext>(x => x.UseSqlite (Configuration.GetConnectionString ("DefaultConnection")));
          // var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Secret phase"));
            IdentityBuilder builder = services.AddIdentityCore<User>(opt=>{
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            //Authentication MiddleWare
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options => {
                    Options.TokenValidationParameters=new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes (Configuration.GetSection ("AppSettings:Token").Value)),
                    ValidateIssuer =false,
                    ValidateAudience =false
                    };
                });
                services.AddAuthorization(options=>{
                    options.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
                    options.AddPolicy("ModeratePhotoRole", policy=>policy.RequireRole("Admin","Moderator"));
                    options.AddPolicy("VipOnly", policy=>policy.RequireRole("VIP"));
                }
                );
            services.AddMvc(options=> {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(option => {
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });            
            services.AddSingleton(typeof(IConverter),new SynchronizedConverter(new PdfTools()));
            services.AddCors();
            services.AddSignalR();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddAutoMapper();
            //Mapper.Reset();
            services.AddTransient<TrailData>();
           // services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IZwajRepository, ZwajRepository>();
            services.AddScoped<LogUserActivity>();            
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env,TrailData trailData ) {
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe:SecretKey").Value);
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler(BuilderExtensions=>
                {
                    BuilderExtensions.Run(async context =>{
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if(error != null)
                    {
                        context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(error.Error.Message);
                    }
                    });                
                });
                // app.UseHsts();
            }
            // app.UseHttpsRedirection();
            trailData.TrailUsers();
            // .Allowcredentials()
            app.UseCors (x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
           // app.UseCors (x => x.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());
            app.UseSignalR(routes => {
                routes.MapHub<ChatHub>("/chat");
            });         
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.Use(async(context,next) =>{
                await next();
                if(context.Response.StatusCode ==404) {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}