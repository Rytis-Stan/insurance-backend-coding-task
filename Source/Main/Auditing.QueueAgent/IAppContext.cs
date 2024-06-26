﻿using Auditing.Persistence;
using Auditing.QueueAgent.IO;
using BuildingBlocks.MessageQueues;
using Microsoft.Extensions.Logging;

namespace Auditing.QueueAgent;

/// <summary>
/// Represents the "root object" of the whole application. It's responsible for constructing
/// and exposing all dependencies that the <see cref="App" /> class uses.
///
/// In an ideal scenario, this class should only expose objects that represents any type of
/// input and/or output, while trying to keep the <see cref="App" /> class pure in the
/// functional programming sense. The context should abstractions for things such as:
/// 1. Input files and output files, or even the whole file system;
/// 2. Console output(s);
/// 3. Random value inputs (generators);
/// 4. Current date and/or time input(s);
/// 5. Databases;
/// 6. Network inputs and outputs;
/// 7. Loggers;
/// 8. etc.
/// </summary>
public interface IAppContext : IDisposable
{
    ILogger Logger { get; }
    IAuditDatabase Database { get; }
    IEnumerable<IReceivingQueue> Queues { get; }
    IConsole Console { get; }
}
