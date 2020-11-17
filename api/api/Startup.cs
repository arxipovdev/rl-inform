using System.Collections.Generic;
using System.Text;
using api.Data;
using api.Models;
using api.Options;
using api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace api
{
    public class Startup
    {
        private string ConnectionString { get; }
        private OpenApiInfo OpenApiInfo { get; }
        private SwaggerOptions SwaggerOptions { get; }
        private JwtOptions JwtOptions { get; }

        public Startup(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            OpenApiInfo = new OpenApiInfo {Title = "Api", Version = "v1"};

            SwaggerOptions = new SwaggerOptions();
            configuration.GetSection(nameof(SwaggerOptions)).Bind(SwaggerOptions);
            JwtOptions = new JwtOptions();
            configuration.GetSection(nameof(JwtOptions)).Bind(JwtOptions);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(op => op.UseSqlite(ConnectionString));
            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();
            services.AddSingleton(JwtOptions);
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtOptions.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParameters);
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });
            services.AddAuthorization();
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", OpenApiInfo);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                var reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"};
                var openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Reference = reference,
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                };
                var openApiSecurityRequirement = new OpenApiSecurityRequirement()
                    {{openApiSecurityScheme, new List<string>()}};
                options.AddSecurityRequirement(openApiSecurityRequirement);
            });
            
            services.AddScoped<IAccountService, AccountService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(op => op.RouteTemplate = SwaggerOptions.Route);
            app.UseSwaggerUI(op => op.SwaggerEndpoint(SwaggerOptions.Endpoint, SwaggerOptions.Description));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}