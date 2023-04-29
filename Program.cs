using Microsoft.EntityFrameworkCore;
using demo_test_api.Data;
using demo_test_api.Utils;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL_TESTER") ?? throw new InvalidOperationException("Connection string 'MSSQL_TESTER' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Bearer <Token>",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
var Configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        
                    };
                });

builder.Services.AddAutoMapper(typeof(UtilAutoMapper));

var origins = Configuration["AllowOrigin"].Split(',');
builder.Services.AddCors(options =>
{
    if (origins.Length > 0)
    {
        options.AddPolicy(name: "AllowOrigin",
        builder =>
        {
            builder.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(option => option.DefaultModelsExpandDepth(-1));
//}

app.UseHttpsRedirection();

app.UseCors(builder =>
{
    if (origins.Length > 0)
    {
        builder
             .WithOrigins(origins)
             .AllowAnyMethod()
             .AllowAnyHeader();
    }
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
