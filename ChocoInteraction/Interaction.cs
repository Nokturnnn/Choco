using System.Threading.Tasks;

namespace ChocoInteraction;

public interface Interaction
{
    public interface IFileRead
    {
        Task<string> ReadFileAsync(string path);
    }
    public interface IFileWrite
    {
        Task<bool> WriteFileAsync(string path, string content);
    }
    public interface IFileAppend
    {
        Task<bool> AppendFileAsync(string path, string content);
    }
    public interface IFileExists
    {
        bool FileExistsAsync(string path);
    }
    public interface IFileDelete
    {
        Task<bool> DeleteFileAsync(string path);
    }
    
    public class FileService : IFileRead, IFileWrite, IFileAppend, IFileExists, IFileDelete
    {
        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                string content = await File.ReadAllTextAsync(filePath);
                return content;
            }
            catch (Exception ex)
            {
                return $"File Read Error : {ex.Message}";
            }
        }
        public async Task<bool> WriteFileAsync(string filePath, string content)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Write Error : {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AppendFileAsync(string filePath, string content)
        {
            try
            {
                await File.AppendAllTextAsync(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Append Error : {ex.Message}");
                return false;
            }
        }
        public bool FileExistsAsync(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Exists Error : {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Delete Error : {ex.Message}");
                return false;
            }
        }
    }
}