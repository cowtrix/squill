using MudBlazor.Utilities;
using Squill.Shared;

namespace Squill.Data;

[ElementDisplay(Icon = "Person")]
public class Character : ElementBase
{
    public Avatar Avatar { get; set; } = new Avatar();
    public string Color { get; set; } = "FFFFFF";
    public string Description { get; set; }

    public override IEnumerable<(string, string)> GetAttributes()
    {
        yield return ("color", Color);
    }
}

public class Avatar
{
    public string URL { get; set; }
}