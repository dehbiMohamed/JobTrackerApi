using System.Text;
using JobTracker.api.Config;
using JobTracker.api.Data;
using JobTracker.api.Interfaces;
using JobTracker.api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
    
).ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "https://salmon-tree-0a9f05303.1.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();
