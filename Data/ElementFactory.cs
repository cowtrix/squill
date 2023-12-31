﻿using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using Squill.Data.ElementComponents;
using Squill.Data.Elements;

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

    public override bool Equals(object? obj)
    {
        return obj is ElementMetaData data &&
               Guid == data.Guid;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Guid);
    }
}

public static class ElementMetaDataExtensions
{

    public static bool IsPinned(this ElementMetaData metaData) => metaData.Attributes.ContainsKey("pin");

    public static bool ShouldTag(this ElementMetaData metaData) => !metaData.Attributes.ContainsKey("notag");

    public static IEnumerable<Guid> GetEntityLinks(this ElementMetaData metaData)
    {
        if (!metaData.Attributes.TryGetValue("links", out var links))
        {
            return null;
        }
        return JsonConvert.DeserializeObject<List<string>>(links).Select(s => Guid.Parse(s));
    }
}

public class ElementFactory
{
    public static IEnumerable<Type> SupportedTypes => new[]
    {
        typeof(Manuscript),
        typeof(Chapter),
        typeof(Character),
        typeof(Timeline),
        typeof(Location),
    };

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
        var meta = JsonConvert.DeserializeObject<ElementMetaData>(File.ReadAllText(path + ".meta"), m_session.SerializerSettings);
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

        var ele = JsonConvert.DeserializeObject(File.ReadAllText(path), type, m_session.SerializerSettings) as T;
        var defaultComponents = type.GetCustomAttributes(typeof(DefaultComponentTypeAttribute), true)?
           .Cast<DefaultComponentTypeAttribute>()
           .FirstOrDefault()?
           .DefaultTypes;
        if (defaultComponents != null)
        {
            foreach (var defaultComponentType in defaultComponents.Where(e => !ele.GetComponents(e).Any()))
            {
                ele.AddComponent(defaultComponentType);
            }
        }
        return ele;
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
        var defaultComponents = type.GetCustomAttributes(typeof(DefaultComponentTypeAttribute), true)?
           .Cast<DefaultComponentTypeAttribute>()
           .FirstOrDefault()?
           .DefaultTypes;
        if (defaultComponents != null)
        {
            foreach (var defaultComponentType in defaultComponents)
            {
                newEle.AddComponent(defaultComponentType);
            }
        }
        var outputPath = Path.Combine(m_session.Project.DataDir, $"{name}.{newEle.GetType().Name.ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(dir))
        {
            outputPath = Path.Combine(m_session.Project.DataDir, dir, name);
        }
        var meta = new ElementMetaData
        {
            Name = name,
            Guid = newEle.Guid.ToString(),
            Path = outputPath,
            Type = type.FullName,
            LastModified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };
        File.WriteAllText(outputPath, JsonConvert.SerializeObject(newEle, m_session.SerializerSettings));
        File.WriteAllText(outputPath + ".meta", JsonConvert.SerializeObject(meta, m_session.SerializerSettings));
        return (meta, newEle);
    }
}
