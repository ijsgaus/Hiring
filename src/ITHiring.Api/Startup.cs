using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ITHiring.Api.Authorization;
using ITHiring.Api.Services;
using ITHiring.Api.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace ITHiring.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            ConfigureAuth(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "ITHiring API", Version = "v1"});
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ITHiring.Api.xml");
                c.IncludeXmlComments(filePath);
                c.AddSecurityDefinition("bearer", new BearerAuthScheme("/api/token"));
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            var signingKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));

            

            services.Configure<TokenProviderOptions>(opt =>
            {
                opt.Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value;
                opt.Audience = Configuration.GetSection("TokenAuthentication:Audience").Value;
                opt.Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value;
                opt.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddScoped<IIdentityResolver, FakeIdentityResolver>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero,
                
            };

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = ctx => Task.CompletedTask
                    };
                })
                .AddCookie(opt =>
                {
                    opt.Cookie.Name = Configuration.GetSection("TokenAuthentication:CookieName").Value;
                    opt.TicketDataFormat = new CustomJwtDataFormat(
                        SecurityAlgorithms.HmacSha256,
                        tokenValidationParameters);
                    opt.Events = new CookieAuthenticationEvents
                    {
                        
                    };
                    
                });


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseMiddleware<TokenProviderMiddleware>();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API"));
        }
    }
}
