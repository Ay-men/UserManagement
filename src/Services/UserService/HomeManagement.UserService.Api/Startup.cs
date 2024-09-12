using System.Security.Claims;
using System.Text;
using FluentValidation.AspNetCore;
using HomeManagement.UserService.Application.Commands;
using HomeManagement.UserService.Domain.Interfaces;
using HomeManagement.UserService.Infrastructure.Persistence;
using HomeManagement.UserService.Infrastructure.Repositories;
using HomeManagement.UserService.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace HomeManagement.UserService.Api;

public class Startup
{
  public IConfiguration Configuration { get; }

  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();

    services.AddMediatR(cfg =>
            {
              cfg.RegisterServicesFromAssembly(typeof(CreateUserProfileCommand).Assembly);
              //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

    // Add AutoMapper
    // services.AddAutoMapper(typeof(Startup).Assembly, typeof(CreateUserProfileCommand).Assembly);
    // services.AddAutoMapper(cfg => cfg.AddProfile<AuthMappingProfile>(), typeof(Startup).Assembly);

    // Add DbContext
    services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

    // Add Repositories
    services.AddScoped<IUserProfileRepository, UserProfileRepository>();

    // Add RabbitMQ Consumer as a hosted service
    services.AddHostedService<RabbitMQConsumer>();
    services.AddSwaggerGen(c =>
            {
              c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1", Description = "Description of Users service" });

              // Add JWT Authentication
              c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
              {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
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
            new string[] {}
        }
        });
              var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
              var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
              if (File.Exists(xmlPath))
              {
                c.IncludeXmlComments(xmlPath);
              }
              else
              {
                Console.WriteLine($"XML comments file not found: {xmlPath}");
              }
            });
    // Add Swagger
    // services.AddSwaggerGen(c =>
    // {
    //   c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service API", Version = "v1" });
    // });
    // services.AddServiceDiscovery(o => o.UseEureka());

    var jwtSettings = Configuration.GetSection("Jwt");
    var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
    services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
  .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

      };
    });

    // // Add JWT Authentication
    // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //     .AddJwtBearer(options =>
    //     {
    //       options.TokenValidationParameters = new TokenValidationParameters
    //       {
    //         ValidateIssuer = true,
    //         ValidateAudience = true,
    //         ValidateLifetime = true,
    //         ValidateIssuerSigningKey = true,
    //         ValidIssuer = Configuration["Jwt:Issuer"],
    //         ValidAudience = Configuration["Jwt:Audience"],
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
    //       };
    //     });

    // Add FluentValidation
    // services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserProfileCommandValidator>());
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1"));
    }

    // app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}