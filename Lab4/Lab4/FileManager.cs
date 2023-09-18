namespace Lab4;

internal class FileManager
{
    private readonly string _rootDirectory;

    public FileManager(string rootDirectory)
    {
        _rootDirectory = rootDirectory;
    }

    public bool FileExists(string path)
    {
        var fullPath = _rootDirectory + path;
        return File.Exists(fullPath);
    }

    public async Task<byte[]> ReadFileAsync(string path)
    {
        var fullPath = _rootDirectory + path;
        var content = await File.ReadAllBytesAsync(fullPath);
        return content;
    }
}