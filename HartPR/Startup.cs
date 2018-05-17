using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using HartPR.Entities;
using HartPR.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace HartPR
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
            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            });

            var connectionString = Configuration["connectionStrings:HartPRDBConnectionString"];
            services.AddDbContext<HartPRContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<IHartPRRepository, HartPRRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            //implementing headers
            services.AddHttpCacheHeaders((expirationModelOptions)
                =>
                { expirationModelOptions.MaxAge = 600; },
                (validationModelOptions)
                =>
                { validationModelOptions.AddMustRevalidate = true; }
                );
            //implement caching and optimistic concurrency
            services.AddResponseCaching();

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 1000,
                        Period = "5m"
                    },
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 200,
                        Period = "10s"
                    },
                };
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                .Build());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, HartPRContext hartPRContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

                    });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                // players mapping
                cfg.CreateMap<Entities.Player, Models.PlayerDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"));

                cfg.CreateMap<Models.PlayerDto, Entities.Player>();

                //cfg.CreateMap<Models.AuthorForCreationWithDateOfDeathDto, Entities.Author>();

                cfg.CreateMap<Models.PlayerForCreationDto, Entities.Player>();

                cfg.CreateMap<Models.PlayerForManipulationDto, Entities.Player>();

                cfg.CreateMap<Models.PlayerForUpdateDto, Entities.Player>();

                cfg.CreateMap<Entities.Player, Models.PlayerForUpdateDto>();

                // tournaments mapping
                cfg.CreateMap<Entities.Tournament, Models.TournamentDto>();

                cfg.CreateMap<Models.TournamentDto, Entities.Tournament>();

                cfg.CreateMap<Models.TournamentForCreationDto, Entities.Tournament>();

                cfg.CreateMap<Models.TournamentForManipulationDto, Entities.Tournament>();

                cfg.CreateMap<Models.TournamentForUpdateDto, Entities.Tournament>();

                cfg.CreateMap<Entities.Tournament, Models.TournamentForUpdateDto>();

                // set mapping
                cfg.CreateMap<Entities.Set, Models.SetDtoForDisplay>();

                cfg.CreateMap<Models.SetDto, Entities.Set>();

                cfg.CreateMap<Models.SetForCreationDto, Entities.Set>();

                cfg.CreateMap<Models.SetForManipulationDto, Entities.Set>();

                cfg.CreateMap<Models.SetForUpdateDto, Entities.Set>();

                cfg.CreateMap<Entities.Set, Models.SetForUpdateDto>();

            });

            //comment out this line when doing db migrations
            //hartPRContext.EnsureSeedDataForContext();

            //add first to reject requests.
            app.UseIpRateLimiting();

            //add to authorize
            app.UseAuthentication();

            //add for CORS
            app.UseCors("CorsPolicy");

            //app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseMvc();
        }
    }
}
