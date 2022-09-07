using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        
        builder.Services.AddLogging();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<TodoContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("TodoList")));
        builder.Services.AddHealthChecks().AddSqlite(builder.Configuration.GetConnectionString("TodoList"),tags: new[] { "Liveliness", "Readiness" });
        
        var app = builder.Build();
        
        app.UseForwardedHeaders();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions { Predicate = (check) => check.Tags.Contains("Liveliness") });
            endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = (check) => check.Tags.Contains("Readiness") });
            endpoints.MapControllers();
        });
        

        app.Run();

    }
}