﻿using Claims.AuditDaemon.Configuration;
using Microsoft.Extensions.Configuration;

namespace Claims.AuditDaemon;

public class Program
{
    static void Main()
    {
        new App(AppConfiguration()).Run();
    }

    private static AppConfiguration AppConfiguration()
    {
        return Configuration.AppConfiguration.FromConfiguration(
            // ReSharper disable once StringLiteralTypo
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        );
    }
}
