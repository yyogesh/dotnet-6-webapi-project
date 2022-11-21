using ASPPRODUCT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using ASPPRODUCT.Handler;
using ASPPRODUCT.Container;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var autoMapper = new AutoMapper.MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
AutoMapper.IMapper mapper = autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);

var _logger = new LoggerConfiguration()
.MinimumLevel.Error()
    .WriteTo.File("D:\\Ysquare\\SEB DOTNET CORE\\ASPPRODUCT\\Log\\ProductINfo-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.AddSerilog(_logger);

var _jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtSettings);

// builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var _authKey = builder.Configuration.GetValue<string>("JwtSettings:securityKey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddDbContext<LearnContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("productConString"));
});

builder.Services.AddScoped<IProductContainer, ProductsContainer>();
builder.Services.AddScoped<IRefereshTokenGenerator, RefereshTokenGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
