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
    public bool IsSynchronized { get; set; }
    public event EventHandler OnSynchronized;
    
    private Repository m_repository;
    private Task? m_workerTask;

    public void TryStartLoad()
    {
        if(m_workerTask != null)
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
            Repository.Clone(Project.RepositoryURL, Path.Combine(Project.DataDir, Project.Guid.ToString()), new CloneOptions { CredentialsProvider = GetCredentials });
        }
        m_repository = new Repository(Path.Combine(Project.DataDir, Project.Guid.ToString()));        
        if(!m_repository.Branches.Any(b => b.FriendlyName == MASTER_NAME))
        {
            m_repository.CreateBranch(MASTER_NAME);            
        }
        Commands.Checkout(m_repository, m_repository.Branches.Single(b => b.FriendlyName == MASTER_NAME));
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
        if(url != Project.RepositoryURL)
        {
            throw new Exception();
        }
        return new UsernamePasswordCredentials { Username = "cowtrix", Password = Project.RepositoryToken };
    }
}
