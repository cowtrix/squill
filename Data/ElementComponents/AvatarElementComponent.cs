using Microsoft.AspNetCore.Components;
using Squill.Components;
using Squill.Data.Elements;
using Squill.Shared;

namespace Squill.Data.ElementComponents;

public class AvatarComponent : ElementComponent
{
    public string ImagePath { get; set; }
    public string Color { get; set; }

    public override IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield return ("color", !string.IsNullOrEmpty(Color) ? Color : session.GetMetaData(Owner).Name.GetDefaultColor().ToString());
        yield return ("img", ImagePath);
    }

    public override RenderFragment GetEditor(ProjectSession session, IComponentOwner owner) => builder =>
    {
        var index = 0;
        builder.OpenComponent<ImageField>(index++);
        builder.AddAttribute(index++, "Value", ImagePath);
        builder.AddAttribute(index++, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => ImagePath = s));
        builder.AddAttribute(index++, "Session", session);
        builder.CloseComponent();

        /*builder.OpenElement(index++, "div");
        builder.AddAttribute(index++, "class", "color-picker");
        builder.OpenElement(index++, "div");
       builder.AddAttribute(index++, "class", "color-preview");
       builder.AddAttribute(index++, "style", $"background-color:{(string.IsNullOrEmpty(Color) ? owner.Guid.GetDefaultColor() : new MudColor(Color))}");
       builder.CloseElement();

      builder.OpenComponent<MudColorPicker>(index++);
       builder.AddAttribute(index++, "Label", "Tag Color");
       builder.AddAttribute(index++, "Value", string.IsNullOrEmpty(Color) ? owner.Guid.GetDefaultColor() : new MudColor(Color));
       builder.AddAttribute(index++, "ValueChanged", EventCallback.Factory.Create<MudColor>(this, (s) =>
       {
           var defaultCol = owner.Guid.GetDefaultColor();
           if (defaultCol == s)
           {
               Color = null;
           }
           else
           {
               Color = s.ToString();
           }
       }));
       builder.AddAttribute(index++, "Variant", Variant.Outlined);
       builder.AddAttribute(index++, "IconSize", MudBlazor.Size.Small);
       builder.AddAttribute(index++, "Margin", Margin.Dense);
       builder.CloseComponent();

        builder.CloseElement();*/
    };
}
