using Squill.Data.ElementComponents;
using Squill.Shared;

namespace Squill.Data.Elements;

[ElementDisplay(Icon = "person-bounding-box")]
[DefaultComponentType(
    typeof(DescriptionComponent), 
    typeof(AvatarComponent))]
[ComponentType(
    typeof(ElementLinkComponent), 
    typeof(StringAttributeComponent))]
public class Character : ElementBase
{
    public HashSet<eCharacterType> CharacterTypes { get; set; } = new HashSet<eCharacterType>();
    public override bool ShouldTag => true;
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
