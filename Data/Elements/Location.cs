using Squill.Data.ElementComponents;
using Squill.Shared;

namespace Squill.Data.Elements;

[ElementDisplay(Icon = "LocationPin")]
[DefaultComponentType(
    typeof(DescriptionComponent),
    typeof(AvatarComponent))]
public class Location : ElementBase
{
    public override bool ShouldTag => true;
}