using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using ODataBackend.Data;
using ODataBackend.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<User>("Users");

// Add services
builder.Services.AddDbContext<UserContext>(options =>
    options.UseInMemoryDatabase("UsersDb"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Count()
        .SetMaxTop(1000)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

// CORS için explicit JSON options
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseRouting();
app.MapControllers();

app.Run();
