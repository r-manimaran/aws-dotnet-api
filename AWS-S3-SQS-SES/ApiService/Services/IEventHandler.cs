using ApiService.Models;

namespace ApiService.Services;

public interface IEventHandler<in TEvent> where TEvent : BaseEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
