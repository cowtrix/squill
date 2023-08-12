using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace Squill.Data;

public class ProjectSession
{
    public ProjectSession(Project project)
    {
        Project = project;
    }

    public const string MASTER_NAME = "squill_master";

    public Project Project { get; set; }
    public IEnumerable<IElement> Elements => m_elements.Values;
    public bool IsSynchronized { get; set; }
    public event EventHandler OnSynchronized;

    private string RepositoryPath => Path.Combine(Project.DataDir, Project.Guid.ToString());

    private Repository m_repository;
    private Task? m_workerTask;
    private ElementFactory m_factory;
    private Dictionary<Guid, IElement> m_elements;

    public void TryStartLoad()
    {
        if (m_workerTask != null)
        {
            return;
        }
        m_workerTask = new Task(async () => await LoadAsync());
        m_workerTask.Start();
    }

    private async Task LoadAsync()
    {
        if (!Project.IsConfigured)
        {
            throw new Exception("Project not configured");
        }
        if (!Directory.Exists(Project.GetPath()))
        {
            Repository.Clone(Project.RepositoryURL, RepositoryPath, new CloneOptions { CredentialsProvider = GetCredentials });
        }
        m_repository = new Repository(RepositoryPath);
        if (!m_repository.Branches.Any(b => b.FriendlyName == MASTER_NAME))
        {
            m_repository.CreateBranch(MASTER_NAME);
        }
        Commands.Checkout(m_repository, m_repository.Branches.Single(b => b.FriendlyName == MASTER_NAME));

        m_elements = Directory.GetFiles(RepositoryPath, "*.meta", SearchOption.AllDirectories)
            .Select(p => m_factory.GetElement(p))
            .ToDictionary(e => e.Guid, e => e);

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
    }

    private Credentials GetCredentials(string url, string usernameFromUrl, SupportedCredentialTypes types)
    {
        if (url != Project.RepositoryURL)
        {
            throw new Exception();
        }
        return new UsernamePasswordCredentials { Username = "cowtrix", Password = Project.RepositoryToken };
    }
}
