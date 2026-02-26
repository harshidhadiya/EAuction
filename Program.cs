using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MACUTION.Data;
using MACUTION.Middleware;
using MACUTION.Validators;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Name;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PasswordHasher<Object>, PasswordHasher<object>>();
builder.Services.AddScoped<ItokenGeneration, Tokenget>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreationValidators>();
builder.Services.AddValidatorsFromAssemblyContaining<changePasswordValidators>();
builder.Services.AddValidatorsFromAssemblyContaining<signupValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddDbContext<MacutionDatabase>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddAuthentication(opiton =>
    {
        opiton.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opiton.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("harshidHADIYAHOWAREYOUDFSDFSDSDFGS"))
        };
});
builder.Services.AddAuthorization(policy =>
{
    policy.AddPolicy("admin", opt => opt.RequireRole("ADMIN"));
    policy.AddPolicy("user", option => option.RequireRole("USER"));
});

var app = builder.Build();
// // app.UseRouting();
// app.UseHttpsRedirection();
app.UseExceptionHandler(options => options.Run(async (context) =>
{
    Console.WriteLine(context.Features.GetRequiredFeature<IExceptionHandlerFeature>().Error);
    await context.Response.WriteAsJsonAsync(new {message="error occurs"});
}));
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<MappingId>();
app.MapGet("/", (HttpContext context) => { 
    
    
    return "Hello World!";
}).AddEndpointFilter(async(context, next) =>
{
    var result=await next(context);
    return result;
});
app.MapControllers();
app.Run();
