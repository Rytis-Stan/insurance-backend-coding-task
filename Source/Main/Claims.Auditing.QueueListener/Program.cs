using Claims.Auditing.QueueListener.Configuration;

namespace Claims.Auditing.QueueListener;

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
