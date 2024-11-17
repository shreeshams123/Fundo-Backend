using DataLayer.Data;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Interfaces;

using Models.Entities;
using BusinessLayer.Services;
using DataLayer.Interfaces;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;

using BusinessLayer.Utilities;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;
using DataLayer.Exceptions;




var builder = WebApplication.CreateBuilder(args);
Log.Logger=new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();
if (!Directory.Exists("Logs"))
{
    Directory.CreateDirectory("Logs");
}
builder.Host.UseSerilog();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "FunDoRedisInstance";
});
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FunDoConnection")));
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserDL, UserDL>();
builder.Services.AddScoped<INoteBL, NoteBL>();
builder.Services.AddScoped<INoteDL, NoteDL>();
builder.Services.AddScoped<ICollaboratorBL, CollaboratorBL>();
builder.Services.AddScoped<ICollaboratorDL, CollaboratorDL>();
builder.Services.AddScoped<ILabelBL, LabelBL>();
builder.Services.AddScoped<ILabelDL, LabelDL>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<TokenHelper>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult(); 
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Invalid token. Please log in again.",
                ErrorType = "AuthenticationFailed"
            });

            return context.Response.WriteAsync(result);
        },
        OnChallenge = context =>
        {
            context.HandleResponse(); 
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Token is required. Please provide a valid token.",
                ErrorType = "AuthorizationFailed"
            });

            return context.Response.WriteAsync(result);
        }
    };
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHttpContextAccessor();


builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FunDoo API V1"));
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();