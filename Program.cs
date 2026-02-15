using BackEnd.Context;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.Response;
using BackEnd.Models.Constants;

// Nota aleatoria: Si el codigo http es 401 a 403 redirigir al login

/*************************************************************************************************************/
var builder = WebApplication.CreateBuilder(args);

// JWT configuration
var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["current_user"];
                return Task.CompletedTask;
            }
        };
    });

// CORS configuracion (So ​​that it accepts requests from anywhere)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FreePolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// Custom validation error response
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // 1. Extraemos los errores de forma limpia
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(er => er.ErrorMessage).ToArray()
                );

            // 2. Creamos tu DTO consistente
            var response = new ApiResponseDto
            {
                Success = false,
                Message = ApplicationError.ValidationError.ValidationFailed,
                Data = null,
                Errors = errors // Aquí van los mensajes como "Name is required", etc.
            };

            // 3. Devolvemos el BadRequest con tu objeto
            return new BadRequestObjectResult(response);
        };
    });

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Controller configuration
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddSwaggerGen();

/*************************************************************************************************************/

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
