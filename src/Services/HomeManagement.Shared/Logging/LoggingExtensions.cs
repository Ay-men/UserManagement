using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeManagement.Shared.Logging
{
  public static class LoggingExtensions
  {
    public static void AddSharedLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
    {
      loggingBuilder.ClearProviders();
      loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
      loggingBuilder.AddConsole();
      loggingBuilder.AddDebug();
      loggingBuilder.AddEventSourceLogger();
    }
  }
}
