using ChocoInteraction;

namespace ChocoLog;

public interface ILogger
{
    public (string message, bool success) Log(string message);
}

public class FileLogger : ILogger
{
    private Interaction.IFileAppend _fileAppend  = new Interaction.FileService();
    public (string message, bool success) Log (string message)
    {
        string path = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        try
        {
            _fileAppend.AppendFile(path, $"{DateTime.Now:dd/MM/yyyy HH:mm} - {message}\n");
            return ("", true);
        }
        catch (Exception e)
        {
            return (e.Message, false);
        }
    }
}