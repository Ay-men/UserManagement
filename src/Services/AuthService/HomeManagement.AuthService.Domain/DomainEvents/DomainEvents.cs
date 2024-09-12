using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HomeManagement.AuthService.Domain.DomainEvents
{
  public static class DomainEvents
  {
    private static IServiceProvider _serviceProvider;

    public static void Init(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public static async Task Raise<T>(T args) where T : INotification
    {
      if (_serviceProvider == null) throw new InvalidOperationException("DomainEvents.Init must be called before raising events.");

      using (var scope = _serviceProvider.CreateScope())
      {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(args);
      }
    }
  }
}