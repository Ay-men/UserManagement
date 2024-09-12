using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace HomeManagement.Shared.Swagger
{
  public static class SwaggerServiceExtensions
  {
    public static void AddCommonSwagger(this IServiceCollection services, string apiTitle, string apiVersion = "v1")
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiTitle, Version = apiVersion });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
      });
    }
  }
}
