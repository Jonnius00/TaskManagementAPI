using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagementAPI.Data;
using TaskManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- EF Core (SQLite) ----------
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// ---------- Controllers + JSON (ignore cycles to avoid ref loops) ----------
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.MaxDepth = 32;
});

// ---------- CORS (dev-friendly) ----------
builder.Services.AddCors(o => o.AddPolicy("frontend", p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
));

// ---------- DI ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------- JWT Auth ----------
var jwt = builder.Configuration.GetSection("Jwt"); // needs Key, Issuer, Audience, ExpiresMinutes
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();

// ---------- OpenAPI (Swagger JSON generator) ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "Projects, Tasks, Users (JWT protected)"
    });

    // avoid conflicting schema names
    c.CustomSchemaIds(t => t.FullName);

    // JWT bearer auth in the spec
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ---------- Serve the OpenAPI JSON at a stable path ----------
app.UseSwagger(c =>
{
    // JSON will be at /openapi/v1/openapi.json
    c.RouteTemplate = "openapi/{documentName}/openapi.json";
});

// ---------- Pipeline ----------
app.UseStaticFiles();          // serves /swagger/index.html from wwwroot/swagger
// app.UseHttpsRedirection();  // keep off unless you configure HTTPS/ports

app.UseCors("frontend");       // before auth for preflight
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Convenience redirects
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));   // root -> docs
app.MapGet("/docs", () => Results.Redirect("/swagger/index.html"));// /docs -> docs

app.Run();
