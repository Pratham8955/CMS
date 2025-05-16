using System.Text;
using CMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CmsproContext>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("DbCon")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OTP sending and Storing functionality
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(op =>
{
    op.IdleTimeout = TimeSpan.FromMinutes(5);
    op.Cookie.HttpOnly = true;
    op.Cookie.IsEssential = true;
    op.Cookie.SameSite = SameSiteMode.None;
    op.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(op =>
    {
        op.RequireHttpsMetadata = false;
        op.SaveToken = true;
        op.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            RoleClaimType = "Role",
        };
    });

// ✅ Fix: Only use ONE CORS policy (AllowFrontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000", "http://10.0.2.2:5291")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
