using Squill.Shared;

namespace Squill.Data;

[ElementDisplay(Icon = "LibraryBooks")]
public class Manuscript : ElementBase
{
    public override bool ShouldTag => false;
    public List<string> Chapters { get; set; } = new List<string>();
}
