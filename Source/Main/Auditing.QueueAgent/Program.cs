using Auditing.QueueAgent.Configuration;

namespace Auditing.QueueAgent;

public class Program
{
    static void Main()
    {
        new App(
            new AppContext(
                AppConfiguration.FromAppSettings()
            )
        ).Run();
    }
}
