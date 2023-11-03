using ChocoInteraction;
using System;
using System.Threading.Tasks;

namespace ChocoLog;

public interface ILogger
{
    Task<(string message, bool success)> LogAsync(string message);
}

public class FileLogger : ILogger
{
    private Interaction.IFileAppend _fileAppend = new Interaction.FileService();
    public async Task<(string message, bool success)> LogAsync(string message)
    {
        string path = "/Users/thomas/Documents/RPI/2023-2025/DEV/Choco/ChocoLog/log.txt";
        try
        {
            await _fileAppend.AppendFileAsync(path, $"{DateTime.Now:dd/MM/yyyy HH:mm} - {message}\n");
            return ("", true);
        }
        catch (Exception e)
        {
            return (e.Message, false);
        }
    }
}