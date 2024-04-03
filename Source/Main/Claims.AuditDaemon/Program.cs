using Claims.Auditing;

namespace Claims.AuditDaemon;

public class Program
{
    static void Main(string[] args)
    {
        // TODO: Move the queue name (and some options???) to the configuration file!
        using var messageQueues = new UninitializedRabbitMqMessageQueues().Initialize();

        Console.WriteLine("Starting to listed to messages.");

        messageQueues.ReceivingQueue().OnReceived(message =>
            Console.WriteLine($"RECEIVED_AT: {DateTime.UtcNow}, MESSAGE: {message}")
        );

        Console.WriteLine("Press [enter] to quit.");
        Console.ReadLine();
    }
}
