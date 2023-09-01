using MudBlazor.Utilities;
using Squill.Shared;
using System.Dynamic;

namespace Squill.Data;

public interface IImageProviderElement : IElement
{
    public string ImagePath { get; }
}

[ElementDisplay(Icon = "Person")]
public class Character : ElementBase, IImageProviderElement
{
    public string Color { get; set; }
    public string Description { get; set; }
    public HashSet<eCharacterType> CharacterTypes { get; set; } = new HashSet<eCharacterType>();
    public override bool ShouldTag => true;
    public string ImagePath { get; set; }

    public override IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield return ("color", !string.IsNullOrEmpty(Color) ? Color : session.GetMetaData(Guid).Name.GetDefaultColor().ToString());
    }
}

public enum eCharacterType
{
    Protagonist,
    Deuteragonist,
    Tritagonist,
    Antagonist,
    Supporting,
    Custom,

    Uncategorised = 128,
}