using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Realtorist.DataAccess.Abstractions;
using Realtorist.Models.Events;
using Realtorist.Services.Abstractions.Events;

namespace Realtorist.Services.Implementations.Default.Events
{
    /// <summary>
    /// Describes event logger
    /// </summary>
    public class EventLogger : IEventLogger
    {
        public event EventHandler<Event> EventCreated;

        private readonly IEventsDataAccess _eventsDataAccess;
        private readonly ILogger _logger;

        public EventLogger(IEventsDataAccess eventsDataAccess, ILogger<EventLogger> logger)
        {
            _eventsDataAccess = eventsDataAccess ?? throw new ArgumentNullException(nameof(eventsDataAccess));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> CreateEventAsync(EventLevel eventLevel, string eventType, string title, string message)
        {
            var eventToAdd = new Event
            {
                CreatedAt = DateTime.UtcNow,
                Level = eventLevel,
                Type = eventType,
                Title = title,
                Message = message
            };

            eventToAdd.Id = await _eventsDataAccess.CreateEventAsync(eventToAdd);

            _logger.LogInformation($"New event with id '{eventToAdd.Id}' was created.");

            EventCreated?.Invoke(this, eventToAdd);

            return eventToAdd.Id;
        }

        public async Task<Guid> CreateEventAsync(string eventType, string title, string message, Exception exception)
        {
            return await CreateEventAsync(EventLevel.Error, eventType, title, $"{message}\nException: {exception.Message}\nStackTrace:{exception.StackTrace}");
        }
    }
}