namespace ChocoLog;

public interface ILogger
{
    public void Log(string message);
}

// Implemented a message to be logged in a file
public class FileLogger : ILogger
{
    public void Log(string message)
    {
        string path = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        File.AppendAllText(path, $"{DateTime.Now:dd/MM/yyyy HH:mm} - {message}\n");
    }
}