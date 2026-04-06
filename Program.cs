using Serilog;
using SyncSharpServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

//Register Extensions
builder.Services.AddAuthentication(builder.Configuration);

var app = builder.Build();

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
