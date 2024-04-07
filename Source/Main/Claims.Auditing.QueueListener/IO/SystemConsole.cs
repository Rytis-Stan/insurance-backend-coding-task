namespace Claims.Auditing.QueueListener.IO;

public class SystemConsole : IConsole
{
    public void WaitTillEnterKeyPressed()
    {
        Console.ReadLine();
    }
}
