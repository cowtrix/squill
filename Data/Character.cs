using MudBlazor.Utilities;
using Squill.Shared;

namespace Squill.Data;

[ElementDisplay(Icon = "Person")]
public class Character : ElementBase
{
    public Avatar Avatar { get; set; } = new Avatar();
    public string Color { get; set; }
    public string Description { get; set; }

    public override bool ShouldTag => true;

    public override IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield return ("color", !string.IsNullOrEmpty(Color) ? Color : session.GetMetaData(Guid).Name.GetDefaultColor().ToString());
    }
}

public class Avatar
{
    public string URL { get; set; }
}