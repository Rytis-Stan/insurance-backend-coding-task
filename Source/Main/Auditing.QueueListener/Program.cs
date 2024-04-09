using Auditing.QueueListener.Configuration;

namespace Auditing.QueueListener;

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
