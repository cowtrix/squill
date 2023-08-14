using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Squill.Data;

public class ElementMetaData
{
    public string Guid { get; set; }
    public string Name { get; set; }    
    public string Type { get; set; }
    public long LastModified { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    [JsonIgnore]
    public string Path { get; set; }
}

public class ElementFactory
{
    public static IEnumerable<Type> SupportedTypes => new[] { typeof(Manuscript) };

    private ProjectSession m_session;
    public ElementFactory(ProjectSession session)
    {
        m_session = session;
    }

    public ElementMetaData GetMetaData(string path)
    {
        if (path.EndsWith(".meta"))
        {
            path = path.Substring(0, path.Length - ".meta".Length);
        }
        var meta = JsonSerializer.Deserialize<ElementMetaData>(File.ReadAllText(path + ".meta"));
        meta.Path = path;
        return meta;
    }

    public IElement GetElementAtPath(string path) => GetElementAtPath<IElement>(path);

    public T GetElementAtPath<T>(string path) where T : class, IElement
    {
        if (!File.Exists(path))
        {
            throw new Exception();
        }
        var meta = GetMetaData(path);
        var type = System.Type.GetType(meta.Type, true);
        if (!typeof(T).IsAssignableFrom(type))
        {
            throw new Exception();
        }
        return JsonSerializer.Deserialize(File.ReadAllText(path), type) as T;
    }

    private string GetAutomaticFilename(Type t, string dir)
    {
        var name = $"Untitled {t.Name}";
        var fullDir = string.IsNullOrWhiteSpace(dir) ? m_session.Project.DataDir : Path.Combine(m_session.Project.DataDir, dir);
        var existingAutoFile = Directory.GetFiles(fullDir, "*.meta")
            .Count(p => Path.GetFileName(p).StartsWith(name));
        if (existingAutoFile > 0)
        {
            name = $"Untitled {t.Name} ({existingAutoFile + 1})";
        }
        return name;
    }

    public (ElementMetaData, IElement) CreateNewElement(Type type, string dir = "", string name = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = GetAutomaticFilename(type, dir);
        }
        var newEle = Activator.CreateInstance(type) as IElement;
        newEle.Name = name;
        var outputPath = Path.Combine(m_session.Project.DataDir, $"{newEle.Name}.{newEle.GetType().Name.ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(dir))
        {
            outputPath = Path.Combine(m_session.Project.DataDir, dir, $"{newEle.Name}");
        }
        var meta = new ElementMetaData
        {
            Name = name,
            Guid = newEle.Guid.ToString(),
            Path = outputPath,
            Type = type.FullName,
            LastModified = DateTimeOffset.UtcNow.Ticks,
        };
        File.WriteAllText(outputPath, JsonSerializer.Serialize(newEle));
        File.WriteAllText(outputPath + ".meta", JsonSerializer.Serialize(meta));
        return (meta, newEle);
    }
}
