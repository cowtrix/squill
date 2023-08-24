using MudBlazor.Utilities;
using Squill.Shared;
using System.Dynamic;

namespace Squill.Data;

[ElementDisplay(Icon = "Person")]
public class Character : ElementBase
{
    public CharacterAvatar Avatar { get; set; } = new CharacterAvatar();
    public string Color { get; set; }
    public string Description { get; set; }
    public HashSet<eCharacterType> CharacterTypes { get; set; } = new HashSet<eCharacterType>();

    public override bool ShouldTag => true;

    public override IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield return ("color", !string.IsNullOrEmpty(Color) ? Color : session.GetMetaData(Guid).Name.GetDefaultColor().ToString());
    }
}

public class CharacterAvatar
{
    public string URL { get; set; }
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