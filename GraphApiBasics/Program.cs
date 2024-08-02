using GraphApiBasics.Interfaces;
using GraphApiBasics.Model;
using GraphApiBasics.Services;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(static options =>
{
    var formatter = options.InputFormatters.OfType<SystemTextJsonInputFormatter>()
        .First(static formatter => formatter.SupportedMediaTypes.Contains("application/json"));

    formatter.SupportedMediaTypes.Add("application/csp-report");
    formatter.SupportedMediaTypes.Add("application/reports+json");
});

builder.Services.AddScoped<IGraphService, GraphService>();

// Adding the secret.json file to the configuration
builder.Configuration.AddJsonFile("secret.json", true, true);

// Registering configuration section as a strongly typed class
builder.Services.Configure<GraphSecretOptions>(builder.Configuration.GetSection("GraphClientConfig"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();