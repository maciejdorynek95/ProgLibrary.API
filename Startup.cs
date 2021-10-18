using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddIdentityCore<Role>();
            services.AddIdentity<User, Role>(options =>
             {
                
                 options.User.RequireUniqueEmail = true;
                 options.Password.RequireDigit = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequiredLength = 0;
                 
             })
             
             .AddEntityFrameworkStores<AuthenticationDbContext>()
             .AddDefaultTokenProviders()
             .AddSignInManager();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "ProgLibrary.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(6000);
                options.Cookie.IsEssential = true;
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
            //services.AddTransient<HttpContext>();
            services.AddHttpContextAccessor();
            services.AddSingleton(AutoMapperConfig.Initialize()); // zwraca IMapper z AutoMapperConfig

            services.AddAuthorization(policies => {
                
                policies.AddPolicy("HasAdminRole", role => role.RequireRole("admin"));
                policies.AddPolicy("HasUserRole", role => role.RequireRole("user"));               
                }
                      
            );
            services.Configure<JwtSettings>(Configuration.GetSection("JWT")); // Bindowanie z sekcji konfiguracji JwtConfig - appsetings.json"   
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true); // poprawa formatowania json
            
            #region PasswordHasher
            //services.Configure<BcryptPasswordHasher>(Configuration.GetSection(BcryptPasswordHasher.Hasher));
            services.AddSingleton<BcryptPasswordHasher>();
            services.AddSingleton<IBcryptPasswordHasherService, BcryptPasswordHasherService>();     

            #endregion

            #region JWT Token 
            services.AddSingleton<IJwtHandler, JwtHandler>(); //JwtBearer Tokens Handler

            JwtSettings setting = new JwtSettings();
            Configuration.Bind("JWT", setting);
            JwtHandler.Settings = setting;
           
            

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
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = setting.Issuer,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey))
                };
                
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
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
