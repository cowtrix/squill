﻿using Squill.Data;
using Squill.Data.Auth;
using System;
using System.Text.Json;

namespace Squill.Services;

public class ProjectService
{
    public static string DataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "squill_data");
    private static Dictionary<Guid, ProjectSession> m_activeSessions = new Dictionary<Guid, ProjectSession>();
    private UserService m_userService;

    public ProjectService(UserService userService)
    {
        m_userService = userService;
    }

    private static string GetProjectMetaPath(string filename) => Path.Combine(DataDirectory, filename);

    public async Task<ProjectSession> GetSession(Guid guid)
    {
        if(m_activeSessions.TryGetValue(guid, out var session))
        {
            return session;
        }
        var project = await GetProject(guid);
        session = new ProjectSession(m_userService.CurrentUser, project);
        m_activeSessions[project.Guid] = session;
        return session;
    }

    public async Task<Project> GetProject(Guid guid)
    {
        var path = GetProjectMetaPath($"{guid}.meta");
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
        File.WriteAllText(GetProjectMetaPath($"{project.Guid}.meta"), JsonSerializer.Serialize(project));
    }

    public IEnumerable<Project> GetAllProjects(User user)
    {
        if(user == null)
        {
            return Enumerable.Empty<Project>();
        }
        if (!Directory.Exists(DataDirectory))
        {
            Directory.CreateDirectory(DataDirectory);
        }
        var allProj = Directory.GetFiles(DataDirectory, "*.meta");
        return allProj
            .Select(p => JsonSerializer.Deserialize<Project>(File.ReadAllText(p)))
            .Where(p => p != null && p.Owner == user.Name);
    }
}
