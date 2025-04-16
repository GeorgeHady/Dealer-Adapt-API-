using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.IdentityUserModels;
using Dealer_Adapt_API.Services;
using Dealer_Adapt_API.Servicesl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// George Hady
/// 000821026
/// Capstone: Dealer Adapt
/// </summary>

// create a new web application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer", //  JwtBearerDefaults.AuthenticationScheme, // "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

    // Add ApiKey security definition
    var apiKeySecurityScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "ApiKey",
        Type = SecuritySchemeType.ApiKey,
        Description = "Enter your API key",
        Reference = new OpenApiReference
        {
            Id = "ApiKey",
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(apiKeySecurityScheme.Reference.Id, apiKeySecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { apiKeySecurityScheme, Array.Empty<string>() }
    });
}
);

// Add DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));


// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Expiration time for email confirmation token sent
//builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
//{
//    options.TokenLifespan = TimeSpan.FromHours(2);
//});

// Add Logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register EmailSender service
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register UserService service
builder.Services.AddTransient<IUserService, UserService>();

// Register JwtService service
builder.Services.AddTransient<IJwtService, JwtService>();

// Register RoleSeeder as a hosted service
builder.Services.AddHostedService<DbInitializer>();

// Configure CORS policy to allow requests from the specified origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("http://localhost:5173", "https://rimush.co")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt"); // Get the Jwt section from appsettings.json

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Add Authorization
builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAdministratorRole",
//               policy => policy.RequireRole("Administrator"));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
}
else
{
    // Production environment
    app.UseExceptionHandler("/Home/Error"); // Custom error handling in production
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles(); // For the wwwroot folder

app.UseRouting();


app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<Dealer_Adapt_API.Middleware.ApiKeyMiddleware>();

app.MapControllers();
//app.MapRazorPages();

app.Run();
