namespace ChocoInteraction;

public class Interaction
{
    public interface IFileRead
    {
        string ReadFile(string path);
    }
    public interface IFileWrite
    {
        bool WriteFile(string path, string content);
    }
    public interface IFileAppend
    {
        bool AppendFile(string path, string content);
    }
    public interface IFileExists
    {
        bool FileExists(string path);
    }
    public interface IFileDelete
    {
        bool DeleteFile(string path);
    }
    public class FileService : IFileRead, IFileWrite, IFileAppend, IFileExists, IFileDelete
    {
        public string ReadFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                return content;
            }
            catch (Exception ex)
            {
                return $"File Read Error : {ex.Message}";
            }
        }
        public bool WriteFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Write Error : {ex.Message}");
                return false;
            }
        }
        public bool AppendFile(string filePath, string content)
        {
            try
            {
                File.AppendAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File Append Error : {ex.Message}");
                return false;
            }
        }
        public bool FileExists(string filePath)
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
        public bool DeleteFile(string filePath)
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