using Squill.Shared;

namespace Squill.Data;

[ElementDisplay(Icon = "Person")]
public class Character : ElementBase
{
    public Avatar Avatar { get; set; } = new Avatar();
    public string Description { get; set; }
}

public class Avatar
{
    public string URL { get; set; }
}