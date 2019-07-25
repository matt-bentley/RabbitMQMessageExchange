using MessageExchange.Core;
using MessageExchange.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
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

            var tasks = new List<Task>()
            {
                Task.Factory.StartNew(async () => await SubscribeAsync()),
                Task.Factory.StartNew(async () => await SubscribeAsync())
            }.ToArray();

            Task.WaitAll(tasks);
        }

        private static async Task SubscribeAsync()
        {
            string id = Guid.NewGuid().ToString();
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
                    var json = Encoding.UTF8.GetString(eventArgs.Body);
                    var @event = JsonConvert.DeserializeObject<TestPublishedEvent>(json);
                    Console.WriteLine($"{id} processed: {@event.TestField}");

                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
                await Task.Delay(10000);
            }
        }
    }
}
