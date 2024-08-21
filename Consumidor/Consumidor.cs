using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;

class Program
{
    static IConnectionFactory connectionFactory = null!;
    static IConnection connection = null!;
    static ISession session = null!;
    static IDestination destination = null!;
    static IMessageConsumer consumer = null!;

    static void Main(string[] args)
    {
        // Conectar ao ActiveMQ
        ConnectActiveMQ();

        Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

        // Iniciar consumo de mensagens
        consumer.Listener += new MessageListener(OnMessageReceived);

        // Manter o aplicativo rodando para receber mensagens
        Console.ReadLine();

        connection.Close();
    }

    static void ConnectActiveMQ()
    {
        connectionFactory = new ConnectionFactory("tcp://localhost:61616");
        connection = connectionFactory.CreateConnection();
        connection.Start();
        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        destination = session.GetQueue("temperature_queue");
        consumer = session.CreateConsumer(destination);
    }

    static void OnMessageReceived(IMessage message)
    {
        if (message is ITextMessage textMessage)
        {
            string body = textMessage.Text;
            Console.WriteLine($" [x] Received {body}");
        }
    }
}
