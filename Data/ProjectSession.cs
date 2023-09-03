using LibGit2Sharp;
using Squill.Data.Auth;
using Newtonsoft.Json;
using Squill.Data.Elements;

namespace Squill.Data;

public class ProjectSession
{
    public ProjectSession(User user, Project project)
    {
        Project = project;
        m_user = user;
        m_factory = new ElementFactory(this);
        m_elementCache = new Dictionary<Guid, IElement>();
        SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
        };
    }

    public JsonSerializerSettings SerializerSettings { get; init; }
    public const string MASTER_NAME = "squill_master";

    public Project Project { get; set; }
    public string CurrentDirectory { get; set; } = "";
    public IEnumerable<ElementMetaData> ElementMeta => m_elementMetadata.Values;
    public bool IsSynchronized { get; set; }
    public event EventHandler OnSynchronized;

    private User m_user;
    private Repository m_repository;
    private Task? m_workerTask;
    private ElementFactory m_factory;
    private Dictionary<Guid, ElementMetaData> m_elementMetadata;
    private Dictionary<Guid, IElement> m_elementCache;


    public void TryStartSynchronize()
    {
        if (m_workerTask != null)
        {
            return;
        }
        m_workerTask = new Task(async () => await SynchronizeAsync());
        m_workerTask.Start();
    }

    private async Task SynchronizeAsync()
    {
        if (!Project.IsConfigured)
        {
            throw new Exception("Project not configured");
        }
        if (!Directory.Exists(Project.DataDir))
        {
            Repository.Clone(Project.RepositoryURL, Project.DataDir, new CloneOptions { CredentialsProvider = GetCredentials });
        }
        m_repository = new Repository(Project.DataDir);
        if (!m_repository.Branches.Any(b => b.FriendlyName == MASTER_NAME))
        {
            m_repository.CreateBranch(MASTER_NAME);
        }
        Commands.Checkout(m_repository, m_repository.Branches.Single(b => b.FriendlyName == MASTER_NAME));

        m_elementMetadata = Directory.GetFiles(Project.DataDir, "*.meta", SearchOption.AllDirectories)
            .Select(p => m_factory.GetMetaData(p))
            .ToDictionary(e => Guid.Parse(e.Guid), e => e);

        IsSynchronized = true;
        OnSynchronized?.Invoke(this, null);
        m_workerTask = null;
    }

    public async Task Save()
    {
        if (!IsSynchronized)
        {
            throw new Exception();
        }
        Commands.Stage(m_repository, Directory.GetFiles(Project.DataDir));
    }

    private Credentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        if (url != Project.RepositoryURL)
        {
            throw new Exception();
        }
        return new UsernamePasswordCredentials { Username = m_user.Name, Password = Project.RepositoryToken };
    }

    public async Task<IElement> CreateNewElement(Type t)
    {
        if (!Project.IsConfigured || !IsSynchronized)
        {
            throw new Exception();
        }
        var newElement = m_factory.CreateNewElement(t, CurrentDirectory);
        m_elementMetadata[newElement.Item2.Guid] = newElement.Item1;
        return newElement.Item2;
    }

    public T GetElement<T>(ElementMetaData metaData) where T : class
    {
        if (!m_elementCache.TryGetValue(Guid.Parse(metaData.Guid), out var ele))
        {
            ele = m_factory.GetElementAtPath(metaData.Path);
            m_elementCache[ele.Guid] = ele;
        }
        return (T)ele;
    }

    public ElementMetaData? GetMetaData(Guid guid)
    {
        if (!m_elementMetadata.TryGetValue(guid, out var ele))
        {
            return null;
        }
        return ele;
    }

    public async Task UpdateElement(IElement element)
    {
        var meta = GetMetaData(element.Guid);
        var save = JsonConvert.SerializeObject(element, SerializerSettings);
        if (File.Exists(meta.Path))
        {
            var existing = await File.ReadAllTextAsync(meta.Path);
            if (existing != save)
            {
                await File.WriteAllTextAsync(meta.Path, save);
            }
        }
        foreach (var attr in element.GetAttributes(this).ToDictionary(s => s.Item1, s => s.Item2))
        {
            meta.Attributes[attr.Key] = attr.Value;
        }
        await UpdateMetadata(meta);
    }

    public async Task UpdateMetadata(ElementMetaData meta)
    {
        var metaPath = meta.Path + ".meta";
        var save = JsonConvert.SerializeObject(meta, SerializerSettings);
        if (File.Exists(metaPath))
        {
            var existing = await File.ReadAllTextAsync(metaPath);
            if (existing != save)
            {
                meta.LastModified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                save = JsonConvert.SerializeObject(meta, SerializerSettings);
                await File.WriteAllTextAsync(metaPath, save);
            }
        }
    }

    public async Task RenameElement(ElementMetaData meta, string dir, string newName)
    {
        var newPath = Path.Combine(Path.GetDirectoryName(meta.Path), dir, newName + Path.GetExtension(meta.Path));
        if (meta.Name == newName && meta.Path == newPath)
        {
            return;
        }
        File.Copy(meta.Path, newPath);
        File.Delete(meta.Path);
        File.Delete(meta.Path + ".meta");
        meta.Path = newPath;
        meta.Name = newName;
        await File.WriteAllTextAsync(meta.Path + ".meta", JsonConvert.SerializeObject(meta, SerializerSettings));
    }

    public int GetTypeCount(Type t) => m_elementMetadata.Count(m => m.Value.Type == t.FullName);

    public TreeChanges? GetGitDiff()
    {
        if (m_repository == null)
        {
            return null;
        }
        Commands.Stage(m_repository, "*");
        return m_repository.Diff.Compare<TreeChanges>(m_repository.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory);
    }

    public void Commit(string message, bool userTriggered)
    {
        var sig = new Signature(m_user.Name, m_user.Email, DateTimeOffset.Now);
        if (GetGitDiff().Any())
        {
            m_repository.Commit($"[SYNC] {message}{(userTriggered ? " [Explicit]" : "")}", sig, sig);
        }
        var remote = m_repository.Network.Remotes["origin"];
        var options = new PushOptions();
        options.CredentialsProvider = GetCredentials;
        var pushRefSpec = $"refs/heads/{MASTER_NAME}";
        m_repository.Network.Push(remote, pushRefSpec, options);
    }

    public void TogglePinElement(ElementMetaData element)
    {
        const string pin = "pin";
        if (element.Attributes.ContainsKey(pin))
        {
            element.Attributes.Remove(pin);
        }
        else
        {
            element.Attributes[pin] = "true";
        }
        new Task(async () => await UpdateMetadata(element)).Start();
    }

    public async Task RefreshMetadata()
    {
        m_elementMetadata = Directory.GetFiles(Project.DataDir, "*.meta", SearchOption.AllDirectories)
            .Select(p => m_factory.GetMetaData(p))
            .ToDictionary(e => Guid.Parse(e.Guid), e => e);
        foreach (var meta in m_elementMetadata.Values)
        {
            var element = GetElement<IElement>(meta);
            await UpdateElement(element);
        }
    }

    public void CreateNewFolder(string? name = null)
    {
        Directory.CreateDirectory(Path.Combine(Project.DataDir, CurrentDirectory, name ?? "New Folder"));
    }

    public IEnumerable<string> GetCurrentSubDirectories()
    {
        var targetPath = Path.Combine(Project.DataDir, CurrentDirectory);
        if (!Directory.Exists(targetPath))
        {
            yield break;
        }
        foreach (var subDir in Directory.GetDirectories(Path.Combine(Project.DataDir, CurrentDirectory))
            .Where(f => !System.IO.Path.GetFileName(f).StartsWith('.'))
            .Select(d => System.IO.Path.GetFileName(d.Replace(Project.DataDir + Path.DirectorySeparatorChar, ""))))
        {
            yield return subDir;
        }
    }

}


