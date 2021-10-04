using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Fluent;
using ToDo.API.Context;
using ToDo.API.Middleware;
using ToDo.API.Services;



namespace ToDo.API
{
    public class Startup
    {

       
        private static Task HealthResponseWriter(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";
            var healthStatus = "UP";
            if (result.Status == HealthStatus.Unhealthy)
            {
                healthStatus = "DOWN";
            }

            if (result.Status == HealthStatus.Degraded)
            {
                healthStatus = "DEGRADED";
            }
            var json = new JObject(
                new JProperty("status", healthStatus),
                new JProperty("checks", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));

            return context.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
        
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
               var connectionString = _configuration["connectionStrings:todoConnectionString"];

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                });
                var completedQueueName = _configuration["Queues:taskCompleted"];
                var inProgressQueueName = _configuration["Queues:taskInProgress"];
                var hostName = Environment.GetEnvironmentVariable("QUEUE_HOST");
                if (hostName == null || hostName.Trim().Length == 0)
                {
                    hostName = _configuration["QueueConnection:hostName"];
                }

                var userName = Environment.GetEnvironmentVariable("QUEUE_USER");
                if (userName == null || userName.Trim().Length == 0)
                {
                    userName = _configuration["QueueConnection:userName"];
                }

                var password = Environment.GetEnvironmentVariable("QUEUE_PASSWORD");
                if (password == null || password.Trim().Length == 0)
                {
                    password = _configuration["QueueConnection:password"];
                }
                services.AddControllers();
                services.AddDbContext<ToDoContext>(options => options.UseNpgsql(connectionString));
                services.AddScoped<IToDoRepository, ToDoRepository>();
                services.AddScoped<IChecklistAuditRepository, ChecklistAuditRepository>();
                services.AddSingleton<ITaskHandler>(taskHandler => new TaskHandler(
                    completedQueueName,
                    inProgressQueueName,
                    hostName,
                    userName,
                    password));
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddHealthChecks();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ChecklistExceptionHandlingMiddleware>();
            /* if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }*/

            app.UseStatusCodePages();
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = HealthResponseWriter
                });
            });
  
        }
    }

}