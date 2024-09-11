using System.Text;
using HomeManagement.AuthService.Application.Commands;
using HomeManagement.AuthService.Application.Mapping;
using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.AuthService.Domain.Interfaces;
using HomeManagement.AuthService.Domain.Services;
using HomeManagement.AuthService.Infrastructure.Persistence;
using HomeManagement.AuthService.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using HomeManagement.AuthService.Application.Validators;
using Microsoft.OpenApi.Models;

namespace HomeManagement.AuthService.Api
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      services.AddCors(options =>
                {
                  options.AddPolicy("CorsPolicy",
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });

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

      services.AddControllers()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>());
      services.AddMediatR(cfg =>
              {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
                //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
              });

      // Add AutoMapper
      services.AddAutoMapper(cfg => cfg.AddProfile<AuthMappingProfile>(), typeof(Startup).Assembly);

      // Add TokenService
      services.AddScoped<TokenService>();

      // Add RabbitMQService
      services.AddSingleton<RabbitMQService>();

      // Add UserRepository
      services.AddScoped<IUserRepository, UserRepository>();


      // Add MediatR

      // Add TokenService
      services.AddScoped<TokenService>();

      // Add RabbitMQService
      services.AddSingleton<RabbitMQService>();

      // Add UserRepository
      services.AddScoped<IUserRepository, UserRepository>();

      // Configure Identity
      services.AddIdentity<User, IdentityRole<Guid>>(options =>
      {

        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;  // No special characters required
        options.Password.RequiredLength = 6;              // Minimum length for passwords
        options.Password.RequiredUniqueChars = 1;         // Requires at least 1 unique character

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Lockout for 5 minutes
        options.Lockout.MaxFailedAccessAttempts = 5;                       // Lockout after 5 failed attempts
        options.Lockout.AllowedForNewUsers = true;                         // Lockout enabled for new users

        // User settings
        options.User.RequireUniqueEmail = true;  // Requires unique email for each user

        // Sign-in settings
        options.SignIn.RequireConfirmedEmail = false;  // Whether email confirmation is required for sign-in
        options.SignIn.RequireConfirmedPhoneNumber = false;  // Whether phone number confirmation is required for sign-in


      })
      .AddEntityFrameworkStores<AuthDbContext>()
      .AddDefaultTokenProviders();

      // Configure JWT authentication
      services.AddAuthentication(options =>
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
          ValidIssuer = Configuration["Jwt:Issuer"],
          ValidAudience = Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
        };
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
      // using (var scope = app.ApplicationServices.CreateScope())
      // {
      //   var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
      //   // await dbContext.Database.EnsureCreatedAsync();
      //   dbContext.Database.Migrate(); // Apply any pending migrations
      //   var u = dbContext.Users.ToList();
      //   var tt = "ssd";
      // }
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseSwagger();
      // logger.LogInformation("Swagger middleware added");

      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth.API V1");
        // logger.LogInformation("Swagger UI configured with endpoint: /swagger/v1/swagger.json");
      });
      app.UseCors("CorsPolicy");

      // app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });


      // Initialize DomainEvents
      Domain.DomainEvents.DomainEvents.Init(app.ApplicationServices);

      // Seed Roles
      CreateRoles(serviceProvider).Wait();
    }

    private async Task CreateRoles(IServiceProvider serviceProvider)
    {
      var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

      string[] roleNames = { "User", "Admin" }; // You can add more roles if needed

      foreach (var roleName in roleNames)
      {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
          // Create the role and seed it to the database
          var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
          if (!result.Succeeded)
          {
            throw new Exception($"Role '{roleName}' creation failed: {string.Join(", ", result.Errors)}");
          }
        }
      }
    }
  }
}