using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var PORT = Environment.GetEnvironmentVariable("$PORT");
//builder.WebHost.UseUrls("http://0.0.0.0:" + PORT);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
if (!string.IsNullOrEmpty(enableSwagger) && bool.Parse(enableSwagger))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
