using Squill.Data;
using System;
using System.Text.Json;
using static MudBlazor.CategoryTypes;

namespace Squill.Services;

public class ProjectService
{
    private static Dictionary<Guid, ProjectSession> m_activeSessions = new Dictionary<Guid, ProjectSession>();

    private static string GetMetaPath(string filename) => Path.Combine(Project.DataDir, filename);

    public async Task<ProjectSession> GetSession(Guid guid)
    {
        if(m_activeSessions.TryGetValue(guid, out var session))
        {
            return session;
        }
        var project = await GetProject(guid);
        session = new ProjectSession(project);
        m_activeSessions[project.Guid] = session;
        return session;
    }

    public async Task<Project> GetProject(Guid guid)
    {
        var path = GetMetaPath($"{guid}.meta");
        if (!File.Exists(path))
        {
            var newProj = new Project
            {
                Guid = guid,
                Name = "Untitled Project",
            };
            UpdateProject(newProj);
        }
        var project = JsonSerializer.Deserialize<Project>(File.ReadAllText(path));
        return project;
    }

    public async Task UpdateProject(Project project)
    {
        File.WriteAllText(GetMetaPath($"{project.Guid}.meta"), JsonSerializer.Serialize(project));
    }

    public IEnumerable<Project> GetAllProjects()
    {
        if (!Directory.Exists(Project.DataDir))
        {
            Directory.CreateDirectory(Project.DataDir);
        }
        var allProj = Directory.GetFiles(Project.DataDir, "*.meta");
        return allProj
            .Select(p => JsonSerializer.Deserialize<Project>(File.ReadAllText(p)))
            .Where(p => p != null);
    }
}
