namespace Auditing.QueueAgent.IO;

public class SystemConsole : IConsole
{
    public void WaitTillEnterKeyPressed()
    {
        Console.ReadLine();
    }
}
