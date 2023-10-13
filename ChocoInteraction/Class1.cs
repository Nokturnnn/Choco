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
    public class FileService : IFileRead, IFileWrite
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
    }
}