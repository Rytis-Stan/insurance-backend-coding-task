﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Claims.Api.Tests;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        OverrideSettings(builder);
        return base.CreateHost(builder);
    }

    private static void OverrideSettings(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(
            // ReSharper disable once StringLiteralTypo
            x => x.AddJsonFile("appsettings.json")
        );
    }
}
