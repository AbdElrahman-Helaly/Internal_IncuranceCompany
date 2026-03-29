using System.Text;
using System.Linq;
using MCIApi.API.Middleware;
using MCIApi.Application.Auth.Interfaces;
using MCIApi.Application.Batches.Interfaces;
using MCIApi.Application.Branches.Interfaces;
using MCIApi.Application.Categories.Interfaces;
using MCIApi.Application.Claims.Interfaces;
using MCIApi.Application.Clients.Interfaces;
using MCIApi.Application.Departments.Interfaces;
using MCIApi.Application.Employees.Interfaces;
using MCIApi.Application.GeneralPrograms.Interfaces;
using MCIApi.Application.InsuranceCompanies.Interfaces;
using MCIApi.Application.JobTitles.Interfaces;
using MCIApi.Application.Localization;
using MCIApi.Application.Relations.Interfaces;

using MCIApi.Application.Providers.Interfaces;
using MCIApi.Application.MemberInfos.Interfaces;
using MCIApi.Application.MemberPolicies.Interfaces;
using MCIApi.Application.Policies.Interfaces;
using MCIApi.Application.Approvals.Interfaces;
using MCIApi.Application.CPTs.Interfaces;
using MCIApi.Application.Medicines.Interfaces;
using MCIApi.Application.Units.Interfaces;
using MCIApi.Application.Common.Interfaces;
using MCIApi.Domain.Abstractions;
using MCIApi.Application.Sms;
using MCIApi.Application.TokenBlacklist;
using MCIApi.Domain.Localization;
using MCIApi.Infrastructure.Localization;
using MCIApi.Infrastructure.Persistence;
using MCIApi.Infrastructure.Services;
using MCIApi.Infrastructure.Services.TokenBlacklist;
using MCIApi.Infrastructure.Sms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using MCIApi.API.Filters;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Set EPPlus license globally for non-commercial use
ExcelPackage.License.SetNonCommercialPersonal("MCI API");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ILocalizationHelper, LocalizationHelper>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "ar" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

// Caching and HTTP
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ISmsService, SmsService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

// Application services
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IJobTitleService, JobTitleService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRelationService, RelationService>();
builder.Services.AddScoped<IGeneralProgramService, GeneralProgramService>();
// ProviderCategoriesController removed; categories are exposed via ProviderController
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IInsuranceCompanyService, InsuranceCompanyService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMemberInfoService, MemberInfoService>();
builder.Services.AddScoped<IMemberPolicyService, MemberPolicyService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ICPTService, CPTService>();
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<IUnitService, UnitService>();
// ProviderLocationsController removed; keep all location logic inside ProviderController
builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Authentication & JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // No clock skew tolerance
    };

    // Handle authentication failures (including expired tokens)
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = async context =>
        {
            if (context.Exception is Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var lang = context.Request.RouteValues["lang"]?.ToString() ?? "en";
                var message = lang == "ar" ? "انتهت صلاحية الرمز المميز" : "Token has expired";
                var response = System.Text.Json.JsonSerializer.Serialize(new { 
                    message = message,
                    code = "TokenExpired",
                    error = "Token has expired. Please login again."
                });
                await context.Response.WriteAsync(response);
                context.NoResult(); // Stop further processing
            }
            else if (context.Exception is Microsoft.IdentityModel.Tokens.SecurityTokenException)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var lang = context.Request.RouteValues["lang"]?.ToString() ?? "en";
                var message = lang == "ar" ? "رمز مميز غير صالح" : "Invalid token";
                var response = System.Text.Json.JsonSerializer.Serialize(new { 
                    message = message,
                    code = "InvalidToken",
                    error = "Invalid token provided."
                });
                await context.Response.WriteAsync(response);
                context.NoResult();
            }
        },
        OnChallenge = async context =>
        {
            // Handle challenge (when no token is provided or token is invalid)
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var lang = context.Request.RouteValues["lang"]?.ToString() ?? "en";
                var message = lang == "ar" ? "غير مصرح به" : "Unauthorized";
                var response = System.Text.Json.JsonSerializer.Serialize(new { 
                    message = message,
                    code = "Unauthorized",
                    error = "Authentication required. Please provide a valid token."
                });
                await context.Response.WriteAsync(response);
            }
            context.HandleResponse();
        }
    };
}); 

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Alternative: Named policy for specific origins (uncomment and configure as needed)
    /*
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    */
});

// Controllers & Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    })
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MCI API (Clean)",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT token as: **Bearer {your_token}**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // Schema filter to handle IFormFile types
    c.SchemaFilter<FileUploadSchemaFilter>();
    
    // Parameter filter to prevent Swagger from generating parameters for [FromForm] IFormFile
    // This must run before operation filters to prevent parameter generation errors
    c.ParameterFilter<FileUploadParameterFilter>();
    
    // Custom operation filter to handle file uploads
    // This should run after parameter filter to add file parameters to request body
    c.OperationFilter<FileUploadOperationFilter>();
    
    // Handle IFormFile parameters for Swagger
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
    
    // Enable annotations
    c.EnableAnnotations();
});

var app = builder.Build();

// Apply pending migrations automatically on startup
// This ensures the database is up-to-date when the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var configuration = services.GetRequiredService<IConfiguration>();
    
    // Check if auto-migration is enabled (default: true)
    var autoMigrateEnabled = configuration.GetValue<bool>("Database:AutoMigrate", true);
    var failOnMigrationError = configuration.GetValue<bool>("Database:FailOnMigrationError", false);
    
    if (!autoMigrateEnabled)
    {
        logger.LogInformation("Auto-migration is disabled. Skipping database migration check.");
    }
    else
    {
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            
            logger.LogInformation("Checking for pending database migrations...");
            
            // Ensure database exists
            if (!context.Database.CanConnect())
            {
                logger.LogWarning("Cannot connect to database. Attempting to create database...");
                context.Database.EnsureCreated();
            }
            
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migration(s). Applying migrations...", pendingMigrations.Count);
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("  - {Migration}", migration);
                }
                
                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No pending migrations.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            
            // If configured to fail on migration error, throw to prevent app startup
            if (failOnMigrationError)
            {
                logger.LogCritical("Application startup aborted due to migration failure. Set 'Database:FailOnMigrationError' to false to allow startup despite migration errors.");
                throw;
            }
            else
            {
                logger.LogWarning("Application will continue to start despite migration errors. Review logs and apply migrations manually if needed.");
            }
        }
    }
}

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseCors();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

