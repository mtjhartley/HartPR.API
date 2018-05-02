﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Entities;
using HartPR.Services;
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
                cfg.CreateMap<Entities.Set, Models.SetDto>();

            });

            //comment out this line when doing db migrations
            hartPRContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
