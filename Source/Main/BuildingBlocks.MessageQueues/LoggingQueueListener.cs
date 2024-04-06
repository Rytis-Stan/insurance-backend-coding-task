﻿namespace BuildingBlocks.MessageQueues;

public class LoggingQueueListener<TMessage> : IQueueListener<TMessage>
{
    public void OnMessageReceived(TMessage message)
    {
        // TODO: Make the console writer an injectable dependency or change this into an actual log instance.
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd}]. Received message: {message}");
    }
}