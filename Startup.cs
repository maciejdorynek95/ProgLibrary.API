using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProgLibrary.Core.DAL;
using ProgLibrary.Core.Domain;
using ProgLibrary.Core.Repositories;
using ProgLibrary.Infrastructure.Mappers;
using ProgLibrary.Infrastructure.Repositories;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.Services.JwtToken;
using ProgLibrary.Infrastructure.Services.PasswordHashers;
using ProgLibrary.Infrastructure.Settings.JwtToken;
using System;
using System.Text;

namespace ProgLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
             {

                 options.User.RequireUniqueEmail = true;
                 options.Password.RequireDigit = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequiredLength = 0;


             })

             .AddEntityFrameworkStores<AuthenticationDbContext>()
             .AddDefaultTokenProviders();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = "ProgLibraryAPI.Session";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.IOTimeout = TimeSpan.FromSeconds(60);

            });


            #region DBContext
            services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("LibraryDBContext"), options => options.MigrationsAssembly("ProgLibrary.Core")));
            services.AddDbContext<LibraryDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("LibraryDBContext"), options => options.MigrationsAssembly("ProgLibrary.Core")));
            #endregion

            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IdentityUserRole<Guid>>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AspNetUserManager<User>>();
            services.AddHttpContextAccessor();
            services.AddSingleton(AutoMapperConfig.Initialize()); // zwraca IMapper z AutoMapperConfig



            services.AddAuthorization(policies =>
            {

                policies.AddPolicy("HasAdminRole", role => role.RequireRole("admin"));
                policies.AddPolicy("HasUserRole", role => role.RequireRole("user"));
                policies.AddPolicy("HasSuperAdminRole", role => role.RequireRole("superadmin"));

                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);
                policies.DefaultPolicy = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().Build();
                
            });
            services.Configure<JwtSettings>(Configuration.GetSection("JWT")); // Bindowanie z sekcji konfiguracji JwtConfig - appsetings.json"   
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true); // poprawa formatowania json

            #region PasswordHasher
            //services.Configure<BcryptPasswordHasher>(Configuration.GetSection(BcryptPasswordHasher.Hasher));
            services.AddSingleton<BcryptPasswordHasher>();
            services.AddSingleton<IBcryptPasswordHasherService, BcryptPasswordHasherService>();

            #endregion

            #region JWT Token 
            services.AddSingleton<IJwtHandler, JwtHandler>();

            #endregion

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.Audience = Configuration["JWT:Audience"];
                options.Authority = Configuration["JWT:Authority"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]))
                };
            })
            .AddJwtBearer("Azure", options =>
             {
                 options.RequireHttpsMetadata = false;
                 options.Audience = Configuration["JWT:Audience"];
                 options.Authority = "https://login.microsoftonline.com/eb971100-6f99-4bdc-8611-1bc8edd7f436/"; //Dodac Guid 
             });



            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProgLibrary.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "W celu dodania Tokenu JWT wpisz: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",


                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProgLibrary.API v1"));
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseCors();
            //app.UseCookiePolicy();
            //app.UseHsts();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
