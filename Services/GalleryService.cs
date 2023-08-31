using Squill.Data;
using System.Collections;
using System.Xml.Linq;

namespace Squill.Services;

public class GalleryService
{
    public ProjectService ProjectService { get; set; }
    public GalleryService(ProjectService projectService)
    {
        ProjectService = projectService;
    }

    public void SaveNewImage(ProjectSession session, string name, byte[] data)
    {
        var img = Image.Load<Rgba32>(data);
        var path = Path.Combine(session.Project.DataDir, ".img", name);
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        img.Save(path);
    }

    public MemoryStream GetImage(ProjectSession session, string name)
    {
        var fullPath = Path.Combine(session.Project.DataDir, ".img", name);
        if (!File.Exists(fullPath))
        {
            return null;
        }
        var img = File.ReadAllBytes(fullPath);
        var ms = new MemoryStream(img);
        return ms;
    }
}
