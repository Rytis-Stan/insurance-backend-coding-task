namespace Claims.Auditing.QueueListener;

public class SystemConsole : IConsole
{
    public void WaitTillEnterKeyPressed()
    {
        Console.ReadLine();
    }
}
