using MessageExchange.Core;
using MessageExchange.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.Subscriber
{
    class Program
    {
        private const string EXCHANGE_NAME = "message_exchange";
        // the event name is used as the route key
        private const string EVENT_NAME = "TestPublishedEvent";
        private const string QUEUE_NAME = "Subscriber.TestPublishedEvent";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Subscriber...");

            var task = Task.Factory.StartNew(async () =>
            {
                await SubscribeAsync();
            });
            task.Wait();
        }

        private static async Task SubscribeAsync()
        {
            var connectionFactory = Helper.CreateFactory();
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Helper.SetupSubscription(channel, EXCHANGE_NAME, QUEUE_NAME, EVENT_NAME);
                channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (sender, eventArgs) =>
                {
                    var eventName = eventArgs.RoutingKey;
                    var json = Encoding.ASCII.GetString(eventArgs.Body);
                    var @event = JsonConvert.DeserializeObject<TestPublishedEvent>(json);
                    Console.WriteLine($"Processed: {@event.TestField}");

                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
                await Task.Delay(10000);
            }
        }
    }
}
