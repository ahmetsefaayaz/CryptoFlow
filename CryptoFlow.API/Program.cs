using System.Text;
using CryptoFlow.API.Middlewares;
using CryptoFlow.Application.FluentValidation.AuthValidation;
using CryptoFlow.Application.FluentValidation.DepositValidations;
using CryptoFlow.Application.FluentValidation.OrderValidations;
using CryptoFlow.Application.Interfaces.IAuthentication;
using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IDashboard;
using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Application.Interfaces.ITransaction;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IUser;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Application.Services.AuthenticationServices;
using CryptoFlow.Application.Services.CoinServices;
using CryptoFlow.Application.Services.CryptoServices;
using CryptoFlow.Application.Services.DashboardServices;
using CryptoFlow.Application.Services.OrderServices;
using CryptoFlow.Application.Services.TransactionServices;
using CryptoFlow.Application.Services.UserServices;
using CryptoFlow.Application.Services.WalletItemServices;
using CryptoFlow.Application.Services.WalletServices;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Infrastructure.SeedData;
using CryptoFlow.Persistence.DbContexts;
using CryptoFlow.Persistence.Repositories.MongoDb;
using CryptoFlow.Persistence.Repositories.SqlRepositories;
using CryptoFlow.Persistence.Repositories.UnitOfWork;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var dbSettings = builder.Configuration.GetSection("MongoDbSettings");
var connectionString = dbSettings["ConnectionString"];
var databaseName = dbSettings["DatabaseName"];
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});
builder.Services.AddDbContext<CryptoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped,
    ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<CryptoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning() 
    .WriteTo.Console()
    .WriteTo.MongoDB(
        databaseUrl: $"{connectionString}{databaseName}",
        collectionName: "SystemLogs") 
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});
builder.Services.AddHttpClient("CoinGecko", client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
    client.DefaultRequestHeaders.Add("User-Agent", "CryptoFlowApp"); 
});

builder.Services.AddIdentity<User, Role>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<CryptoDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "j_ec:KnW\"!O3v2Hf@Mzq5ks.Pgn{D_A=Umuj@54o6KxN\"pMQ+nX7'Oj)!QW,lN8");

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
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryAPIV2", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer {token}' formatında JWT girin",
        Name = "Authorization",
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
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(MongoGenericRepository<>));
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICoinRepository, CoinRepository>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<ICoinService, CoinService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DepositValidatior>();
builder.Services.AddValidatorsFromAssemblyContaining<WithdrawValidatior>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidatior>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var context = services.GetRequiredService<CryptoDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    
    await SeedData.SeedRolesAsync(roleManager);
    await SeedData.SeedAdminAsync(userManager, unitOfWork);
    await SeedData.SeedCoinsAsync(unitOfWork);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();