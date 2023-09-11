using Squill.Data.ElementComponents;
using Squill.Shared;

namespace Squill.Data.Elements;

[ElementDisplay(Icon = "geo-fill")]
[DefaultComponentType(
    typeof(DescriptionComponent),
    typeof(AvatarComponent))]
public class Location : ElementBase
{
    public override bool ShouldTag => true;
}