using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<MemoryCache>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddCors(options => { options.AddPolicy(name: "All", policy => { policy.WithOrigins("*"); }); });

var app = builder.Build();
var configuration = new ConfigurationBuilder().
                    SetBasePath(app.Environment.ContentRootPath).
                    AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).
                    AddJsonFile($"appsettings.{app.Environment.EnvironmentName}.json", optional: true).
                    Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("All");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
