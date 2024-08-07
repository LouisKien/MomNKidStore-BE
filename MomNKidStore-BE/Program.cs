using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MomNKidStore_BE.Business.BackgroundServices.Implements;
using MomNKidStore_BE.Business.BackgroundServices.Interfaces;
using MomNKidStore_BE.Business.MappingProfiles;
using MomNKidStore_BE.Business.Services.Implements;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.Repositories.Implements;
using MomNKidStore_Repository.Repositories.Interfaces;
using MomNKidStore_Repository.UnitOfWorks.Implements;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("http://0.0.0.0:5173");

builder.Services.AddControllers();

// Connection string
builder.Services.AddDbContext<MomNkidStoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Set policy permission for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "1"));
    options.AddPolicy("RequireStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "2"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "3"));
    options.AddPolicy("RequireAdminOrStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "2"));
    options.AddPolicy("RequireAdminOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "3"));
    options.AddPolicy("RequireStaffOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "2", "3"));
    options.AddPolicy("RequireAllRoles", policy => policy.RequireClaim(ClaimTypes.Role, "1", "2", "3"));
});

// Auto mapper
builder.Services.AddAutoMapper(typeof(Program), typeof(MappingProfile));

//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Service containers
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IVoucherOfShopService, VoucherOfShopService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IBlogService, BlogService>();


// Background service containers
builder.Services.AddScoped<IOrderBackgroundService, OrderBackgroundService>();
builder.Services.AddScoped<IProductBackgroundService, ProductBackgroundService>();


// Register in-memory caching
builder.Services.AddMemoryCache();

builder.Services.AddCors();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    }));
builder.Services.AddHangfireServer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MomNKidStore", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MomNKidStore API V1");
        c.RoutePrefix = "api";
    });
    //app.UseSwaggerUI();
}

app.UseHangfireDashboard();

// Schedule the recurring job (Hangfire)
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate(
    "RejectExpiredOrder",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<IOrderBackgroundService>().RejectExpiredOrder(),
    Cron.MinuteInterval(2)
    );

recurringJobManager.AddOrUpdate(
    "RemoveHiddenProductInCustomerCarts",
    () => app.Services.CreateScope().ServiceProvider.GetRequiredService<IProductBackgroundService>().RemoveHiddenProductInCustomerCarts(),
    Cron.MinuteInterval(2)
    );

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
