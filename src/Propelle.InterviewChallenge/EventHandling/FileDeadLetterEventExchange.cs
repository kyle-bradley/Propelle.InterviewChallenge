using System.Text.Json;

namespace Propelle.InterviewChallenge.EventHandling
{
    public class FileDeadLetterEventExchange
    {//This is a rudimentary implementation for a dead-letter queue. Most production level exchanges come with this built-in. 
        private string queueLocation = $"{Environment.CurrentDirectory}\\DeadLetterQueue";
        private char queueSeperator = '#';

        private readonly InMemoryEventExchange _inMemoryEventExchange;

        public FileDeadLetterEventExchange(InMemoryEventExchange inMemoryEventExchange)
        {
            _inMemoryEventExchange = inMemoryEventExchange;
        }

        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : class
        {
            Directory.CreateDirectory(queueLocation);

            var eventType = typeof(TEvent).Name;
            using (var streamWriter = new StreamWriter($"{queueLocation}\\{eventType}{queueSeperator}{Guid.NewGuid()}"))
            {
                var jsonEvent = JsonSerializer.Serialize(@event);
                await streamWriter.WriteAsync(jsonEvent);
            }
        }

        public async Task ConsumeAndPublish<TEvent>()
            where TEvent : class
        {
            var files = Directory.GetFiles(queueLocation);
            
            var eventsToConsume = files.Where(file =>
            {
                var fileName = Path.GetFileName(file);
                var fileComponents = fileName.Split(queueSeperator);
                var isCorrectEventType = fileComponents.Any() && fileComponents.First().Equals(typeof(TEvent).Name);
                return isCorrectEventType;
            }).ToList();

            var eventsProcessedTasks = eventsToConsume.Select(async filePath =>
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    var fileContents = await streamReader.ReadToEndAsync();
                    var eventToProcess = JsonSerializer.Deserialize<TEvent>(fileContents);

                    try
                    {
                        await _inMemoryEventExchange.Publish(eventToProcess);
                    }
                    catch (Exception) //Swallow exception to try again at a future date. Logging would be recommended at this point.
                    { }

                    File.Delete(filePath);
                }
            });

            await Task.WhenAll(eventsProcessedTasks);
        }
    }
}
