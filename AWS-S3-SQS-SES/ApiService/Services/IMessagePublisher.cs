using ApiService.Models;

namespace ApiService.Services;

public interface IMessagePublisher
{
    Task PublishEventAsync<TEvent>(TEvent @event, string queueUrl) where TEvent:BaseEvent;
}
