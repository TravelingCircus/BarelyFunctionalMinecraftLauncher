using System.IO;

namespace BFML._3D;

public abstract class ModelLoader
{
    protected string Path
    {
        get => _path;
        init
        {
            if (!IsValidFile(value)) throw new InvalidDataException($"{value} isn't a valid model file");
            _path = value;
        }
    }
    private readonly string _path;

    protected ModelLoader(string path)
    {
        Path = path;
    }
    
    public abstract Model Load();
    
    protected abstract bool IsValidFile(string path);
    
    protected FileStream GetStream() => File.Open(Path, FileMode.Open);
}