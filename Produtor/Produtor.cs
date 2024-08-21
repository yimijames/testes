using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Threading;

class Program
{
    static IConnectionFactory connectionFactory;
    static IConnection connection;
    static ISession session;
    static IDestination destination;
    static IMessageProducer producer;

    static void Main(string[] args)
    {
        // Conectar ao ActiveMQ
        ConnectActiveMQ();

        try
        {
            Random random = new Random();
            while (true)
            {
                double temperature = random.NextDouble() * (40 - 20) + 20;
                Console.WriteLine($"Current temperature: {temperature:F2} degrees");
                if (temperature >= 35)
                {
                    PublishTemperature(temperature);
                }
                Thread.Sleep(5000); // Pausa de 5 segundos
            }
        }
        finally
        {
            connection.Close();
        }
    }

    static void ConnectActiveMQ()
    {
        connectionFactory = new ConnectionFactory("tcp://localhost:61616");
        connection = connectionFactory.CreateConnection();
        connection.Start();
        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        destination = session.GetQueue("temperature_queue");
        producer = session.CreateProducer(destination);
        producer.DeliveryMode = MsgDeliveryMode.Persistent;
    }

    static void PublishTemperature(double temperature)
    {
        string message = $"Temperature reached {temperature:F2} degrees";
        ITextMessage textMessage = producer.CreateTextMessage(message);
        producer.Send(textMessage);
        Console.WriteLine($" [x] Sent '{message}'");
    }
}
