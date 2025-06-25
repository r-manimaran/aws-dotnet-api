using ApiService.Models;

namespace ApiService.Services;

public interface IMessageHandlerFactory
{
    IEventHandler<TEvent>? GetHandler<TEvent>(IServiceProvider? serviceProvider = null) where TEvent : BaseEvent;
}
