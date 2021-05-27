using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransitTest
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
            services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.AddConsumer<Consumer>(typeof(ConsumerDefinition));
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureJsonSerializer(settings => { settings.DefaultValueHandling = DefaultValueHandling.Include; return settings; });

                    cfg.Host("localhost", "/", hst =>
                    {
                        hst.Username("guest");
                        hst.Password("guest");
                    });
                    cfg.UseDelayedMessageScheduler();
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        e.UseKillSwitch(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 15;
                            cb.ActivationThreshold = 10;
                            cb.RestartTimeout = TimeSpan.FromMinutes(5);
                        });
                        e.UseDelayedRedelivery(r => r.Intervals(
                            TimeSpan.FromMinutes(1),
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(10),
                            TimeSpan.FromMinutes(15),
                            TimeSpan.FromMinutes(20)));
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(1, TimeSpan.FromSeconds(10));
                            //r.Handle<WebException>();
                            //r.Handle<HttpRequestException>();
                            //r.Handle<TimeoutException>();
                            //r.Handle<RequestTimeoutException>();
                        });
                        e.UseInMemoryOutbox();
                        e.ConfigureConsumer<Consumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MassTransitTest", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MassTransitTest v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
