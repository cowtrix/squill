using Squill.Shared;

namespace Squill.Data.Elements;

[ElementDisplay(Icon = "journals")]
public class Manuscript : ElementBase
{
    public override bool ShouldTag => false;
    public List<string> Chapters { get; set; } = new List<string>();
}
