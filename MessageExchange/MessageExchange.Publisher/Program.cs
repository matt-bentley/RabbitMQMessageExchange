using MessageExchange.Core;
using MessageExchange.Core.Events;
using RabbitMQ.Client;
using System;

namespace MessageExchange.Publisher
{
    class Program
    {
        private const string EXCHANGE_NAME = "message_exchange";
        // the event name is used as the route key
        private const string EVENT_NAME = "TestPublishedEvent";
        private const string QUEUE_NAME = "Subscriber.TestPublishedEvent";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Publiser...");

            var connectionFactory = Helper.CreateFactory();
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Helper.SetupSubscription(channel, EXCHANGE_NAME, QUEUE_NAME, EVENT_NAME);

                PublishEvent(channel, new TestPublishedEvent("Test1"));
                PublishEvent(channel, new TestPublishedEvent("Test2"));
                PublishEvent(channel, new TestPublishedEvent("Test3"));
                PublishEvent(channel, new TestPublishedEvent("Test4"));
                PublishEvent(channel, new TestPublishedEvent("Test5"));
            }
        }

        private static void PublishEvent(IModel channel, TestPublishedEvent @event)
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            Console.WriteLine($"Publishing message: {@event.TestField}");

            channel.BasicPublish(
                        exchange: EXCHANGE_NAME,
                        routingKey: EVENT_NAME,
                        mandatory: true,
                        basicProperties: properties,
                        body: @event.Serialize());
        }     
    }
}
