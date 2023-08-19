using Squill.Shared;

namespace Squill.Data;

[ElementDisplay(Icon = "LibraryBooks")]
public class Manuscript : ElementBase
{
    public List<string> Chapters { get; set; } = new List<string>();
}
