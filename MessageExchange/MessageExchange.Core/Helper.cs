using RabbitMQ.Client;

namespace MessageExchange.Core
{
    public static class Helper
    {
        public static ConnectionFactory CreateFactory()
        {
            return new ConnectionFactory()
            {
                HostName = "localhost",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true
            };
        }

        public static void SetupSubscription(IModel channel, string exchangeName, string queueName, string routeKey)
        {
            channel.ExchangeDeclare(exchangeName, "direct");
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, routeKey);
        }
    }
}
