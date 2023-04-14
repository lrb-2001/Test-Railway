using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var PORT = Environment.GetEnvironmentVariable("$PORT");
//builder.WebHost.UseUrls("http://[*]:"+PORT);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
if (!string.IsNullOrEmpty(enableSwagger) && bool.Parse(enableSwagger))
{
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
