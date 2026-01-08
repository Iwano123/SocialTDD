using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SocialTDD.Application.Interfaces;
using SocialTDD.Application.Services;
using SocialTDD.Application.Validators;
using SocialTDD.Infrastructure.Data;
using SocialTDD.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();

// Add Services
builder.Services.AddScoped<IPostService, PostService>();    
builder.Services.AddScoped<ITimelineService, TimelineService>();

// Add Validators
builder.Services.AddScoped<IValidator<SocialTDD.Application.DTOs.CreatePostRequest>, CreatePostRequestValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialTDD API v1");
        c.RoutePrefix = string.Empty; // Swagger UI på root
    });
}
else
{
    // Only redirect to HTTPS in production
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

