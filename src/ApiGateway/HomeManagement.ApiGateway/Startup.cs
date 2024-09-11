using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;

namespace HomeManagement.ApiGateway
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
      services.AddControllers();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

      services.AddOcelot(Configuration)
          .AddEureka();

      services.AddSwaggerForOcelot(Configuration);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseAuthentication();

      app.UseSwagger();
      app.UseSwaggerForOcelotUI(opt =>
      {
        opt.PathToSwaggerGenerator = "/swagger/docs";
      }).UseOcelot().Wait();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}

// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.IdentityModel.Tokens;
// using Ocelot.Cache.CacheManager;
// using Ocelot.DependencyInjection;
// using Ocelot.Middleware;
// using Ocelot.Provider.Eureka;
// using Steeltoe.Discovery.Client;
// using Steeltoe.Discovery.Eureka;
// using Ocelot.DependencyInjection;
// using Ocelot.Middleware;
// using MMLib.SwaggerForOcelot.DependencyInjection;
// using Microsoft.OpenApi.Models;
// using Ocelot.Provider.Eureka;
// using Microsoft.IdentityModel.Tokens;
// using SmartHomeMS.ApiGateway;
// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using SmartHomeMS.Common;
// namespace SmartHomeMS.ApiGateway
// {
//   public class Startup
//   {
//     public Startup(IConfiguration configuration)
//     {
//       Configuration = configuration;
//     }

//     public IConfiguration Configuration { get; }

//     public void ConfigureServices(IServiceCollection services)
//     {
//       services.AddServiceDiscovery(options => options.UseEureka());

//       services.AddCors(options =>
//           {
//             options.AddPolicy("CorsPolicy",
//                   builder => builder
//                       .AllowAnyOrigin()
//                       .AllowAnyMethod()
//                       .AllowAnyHeader());
//           });
//       services.AddOcelot()
//           .AddEureka()
//           .AddCacheManager(x => { x.WithDictionaryHandle(); });
//       services.AddEndpointsApiExplorer();
//       services.AddSwaggerGen(c =>
//       {
//         c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
//         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//         {
//           Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
//           Name = "Authorization",
//           In = ParameterLocation.Header,
//           Type = SecuritySchemeType.ApiKey,
//           Scheme = "Bearer"
//         });

//         c.AddSecurityRequirement(new OpenApiSecurityRequirement
//           {
//                 {
//                     new OpenApiSecurityScheme
//                     {
//                         Reference = new OpenApiReference
//                         {
//                             Type = ReferenceType.SecurityScheme,
//                             Id = "Bearer"
//                         }
//                     },
//                     Array.Empty<string>()
//                 }
//           });
//       });

//       // SwaggerForOcelot configuration
//       services.AddSwaggerForOcelot(Configuration);

//       var jwtSettings = Configuration.GetSection("Jwt");
//       var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

//       // JWT Authentication
//       services.AddAuthentication(schemes =>
//       {
//         schemes.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         schemes.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//       })
//       .AddJwtBearer("Bearer", options =>
//       {
//         options.Authority = "http://auth.api:8080";  // Replace with your auth service
//         options.Audience = "SmartHome";
//         options.RequireHttpsMetadata = false;
//         options.SaveToken = true;
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//           ValidateIssuerSigningKey = true,
//           IssuerSigningKey = new SymmetricSecurityKey(key),
//           ValidateIssuer = true,
//           ValidateAudience = true,
//           ValidIssuer = jwtSettings["Issuer"],
//           ValidAudience = jwtSettings["Audience"],
//           ValidateLifetime = true,
//           ClockSkew = TimeSpan.Zero
//         };
//       });
//     }



//     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//     {

//       // app.UseHttpsRedirection();
//       // app.UseRouting();
//       // app.UseSwaggerForOcelotUI(opt =>
//       // {
//       //   opt.PathToSwaggerGenerator = "/swagger/docs";
//       // }).UseOcelot().Wait();

//       // app.m.MapControllers();

//       // var appSettings = new AppSettings();
//       // app.ApplicationServices.GetService<IConfiguration>()
//       //     .GetSection("AppSettings")
//       //     .Bind(appSettings);
//       app.UseCors("CorsPolicy");

//       // app.UseCors
//       // (b => b
//       //     .AllowAnyOrigin()
//       //     .AllowAnyMethod()
//       //     .AllowAnyHeader()
//       //     .AllowCredentials()
//       // );
//       // app.UseMiddleware<LoggingMiddleware>();
//       app.UseSwaggerForOcelotUI(opt =>
//       {
//         opt.PathToSwaggerGenerator = "/swagger/docs";
//       }).UseOcelot().Wait();
//     }
//   }
// }