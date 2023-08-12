using System.Text.Json.Serialization;

namespace Squill.Data;

public class Project
{
    public static string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "squill_data");

    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string RepositoryURL { get; set; }
    public string RepositoryToken { get; set; }

    [JsonIgnore]
    public bool IsConfigured => !string.IsNullOrEmpty(RepositoryURL) && !string.IsNullOrEmpty(RepositoryToken);

    public string GetPath(string filename = null) => string.IsNullOrEmpty(filename) ? Path.GetFullPath(Path.Combine(DataDir, Guid.ToString())) : Path.GetFullPath(Path.Combine(DataDir, Guid.ToString(), filename));
}
