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
                x.UsingRabbitMq((context, cfg) =>
                {
                    /*
                    var observer = new ReceiveObserver();
                    var pobserver = new SendObserver();
                    cfg.ConnectReceiveObserver(observer);
                    cfg.ConnectSendObserver(pobserver);
                    */

                    cfg.ConfigureJsonSerializer(settings => { settings.DefaultValueHandling = DefaultValueHandling.Include; return settings; });

                    cfg.Host("localhost", "/", hst =>
                    {
                        hst.Username("guest");
                        hst.Password("guest");
                    });
                    cfg.UseDelayedMessageScheduler();
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        /*
                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(massTransitSettings.CircuitBreakerTrackingPeriodInMinutes);
                            cb.TripThreshold = massTransitSettings.CircuitBreakerTripThreshold;
                            cb.ActiveThreshold = massTransitSettings.CircuitBreakerActiveThreshold;
                            cb.ResetInterval = TimeSpan.FromMinutes(massTransitSettings.CircuitBreakerResetIntervalInMinutes);
                        });
                        */
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20)));
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(1, TimeSpan.FromSeconds(5));
                            // r.Handle<DataException>(x => x.Message.Contains("SQL"));
                            // r.Ignore<ArgumentNullException>();
                            // r.Incremental(2, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(6));

                            // r.Handle<WebException>();
                            // r.Handle<HttpRequestException>();
                            // r.Handle<TimeoutException>();
                            // r.Handle<RequestTimeoutException>();
                        });
                        e.UseInMemoryOutbox();
                        e.Batch<QueryObject>(b =>
                        {
                            b.MessageLimit = 20;
                            b.ConcurrencyLimit = 10;
                            b.TimeLimit = TimeSpan.FromSeconds(10);
                            b.Consumer(() => new Consumer());
                        });
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
