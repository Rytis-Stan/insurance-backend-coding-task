﻿namespace Claims.AuditDaemon.Configuration;

public class RabbitMqConfiguration
{
    public string HostName { get; init; }
    public string QueueName { get; init; }
}
