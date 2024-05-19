using AutoMapper;
using ChatAppCore.Entities;
using ChatAppCore.Interfaces;
using ChatAppInfrastructure;
using ChatAppServer.Hubs;
using ChatAppServer.Services;
using ChatAppServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers's services 
builder.Services.AddControllers();
// Add dependencies
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped(typeof(IUserServices), typeof(UserServices));
builder.Services.AddScoped(typeof(IHomeServices), typeof(HomeServices));
builder.Services.AddScoped<IFileManagerServices, FileManagerServices>();

// Add Swagger Interface
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Authoraizition header",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Add Jwt Configurations
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
        };

    });


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//Add SignalR services

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

// Add Cors Policy rules
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

//Add Database configurations
var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");

builder.Services.AddDbContext<DataContext>(
    o => o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<User, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();

// change password options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

var app = builder.Build();

// Insert Seeded data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    context.Database.EnsureCreated();
    DataSeeder.SeedData(context, userManager);
}
// Add Swagger interface to http pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");


app.Run();

//dotnet ef migrations add initialeCreate -s ..\ChatAppServer\ChatAppServer.csproj
//dotnet ef database update -s ..\ChatAppServer\ChatAppServer.csproj
//dotnet ef migrations remove
