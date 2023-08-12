using System.Text.Json;

namespace Squill.Data;

public class MetaFile
{
    public string Guid { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}

public class ElementFactory
{
    private ProjectSession m_session;
    public ElementFactory(ProjectSession session)
    {
        m_session = session;
    }

    public IElement GetElement(string path) => GetElement<IElement>(path);

    public T GetElement<T>(string path) where T: class, IElement
    {
        var meta = JsonSerializer.Deserialize<MetaFile>(File.ReadAllText(path));
        var targetFilePath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        if (!File.Exists(targetFilePath))
        {
            throw new Exception();
        }
        var type = Type.GetType(meta.Type);
        return JsonSerializer.Deserialize(File.ReadAllText(targetFilePath), type) as T;
    }
}
