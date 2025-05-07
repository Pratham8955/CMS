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
    op.IdleTimeout = TimeSpan.FromMinutes(5);  // ✅ Use IdleTimeout instead of IOTimeout
    op.Cookie.HttpOnly = true;
    op.Cookie.IsEssential = true;
    op.Cookie.SameSite = SameSiteMode.None;  // ✅ Required for cross-origin cookies
    op.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ Required for HTTPS
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
        policy.WithOrigins("http://localhost:3000")  // ✅ Use your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());  // ✅ Required for JWT authentication & cookies
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

// ✅ Fix: Apply only "AllowFrontend" policy and move it before Authentication & Authorization
app.UseCors("AllowFrontend");

app.UseSession();  // ✅ Place before Authentication to ensure cookies work

app.UseAuthentication(); // ✅ Added to ensure JWT Authentication is enabled
app.UseAuthorization(); // Add this line


app.MapControllers();

app.Run();
