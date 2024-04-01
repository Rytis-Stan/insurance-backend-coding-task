﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Claims.Api.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                { "CosmosDb:DatabaseName", "ClaimTestDb" },
                { "ConnectionStrings:DefaultConnection", "Server=localhost\\SQLEXPRESS;Database=AuditTestDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" }
            };
            config.AddInMemoryCollection(testConfig);
        });

        return base.CreateHost(builder);
    }
}
