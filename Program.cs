using Serilog;
using SyncSharpServer.Extensions;
using SyncSharpServer.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

//Register Extensions
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddCorsExtension(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowedOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
