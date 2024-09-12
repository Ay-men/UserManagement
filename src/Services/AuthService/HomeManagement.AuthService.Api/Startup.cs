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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using HomeManagement.AuthService.Application.Validators;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Claims;
using HomeManagement.Shared.Auth;
using HomeManagement.Shared.Middlewares;
using HomeManagement.Shared.Swagger;
using HomeManagement.Shared.RabbitMQ;
using HomeManagement.UserService.Application.Interfaces;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
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
      services.AddServiceDiscovery(options => options.UseEureka());
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IRabbitMQService, Shared.RabbitMQ.RabbitMQService>();
      services.AddSingleton<IConnectionFactory, ConnectionFactory>();
      services.AddScoped<ITokenService, TokenService>();
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
      services.AddJwtAuthentication(Configuration);
      services.AddCommonSwagger("Auth API", "v1");
      services.AddControllers()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>());
      services.AddMediatR(cfg =>
              {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
              });
      services.AddAutoMapper(cfg => cfg.AddProfile<AuthMappingProfile>(), typeof(Startup).Assembly);
      services.AddScoped<TokenService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<TokenService>();
      services.Configure<HomeManagement.AuthService.Api.RabbitMQSettings>(Configuration.GetSection("RabbitMQ"));
      services.AddSingleton<IRabbitMQService, RabbitMQService>();
      services.AddIdentity<User, IdentityRole<Guid>>(options =>
      {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
      })
      .AddEntityFrameworkStores<AuthDbContext>()
      .AddDefaultTokenProviders();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth.API V1");
      });
      app.UseCors("CorsPolicy");
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseMiddleware<ErrorHandlingMiddleware>();
      Domain.DomainEvents.DomainEvents.Init(app.ApplicationServices);
      CreateRoles(serviceProvider).Wait();
    }
    private async Task CreateRoles(IServiceProvider serviceProvider)
    {
      var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
      string[] roleNames = { "User", "Admin" };
      foreach (var roleName in roleNames)
      {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
          var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
          if (!result.Succeeded)
          {
            throw new Exception($"Role '{roleName}' creation failed: {string.Join(", ", result.Errors)}");
          }
        }
      }
    }
  }
  public class RabbitMQSettings
  {
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Port { get; set; }
  }
}