using Squill.Shared;
using Microsoft.AspNetCore.Components;
using Squill.Data.Elements;
using Squill.Components.Editors;

namespace Squill.Data.ElementComponents;

[ElementDisplay(Name = "Description")]
public class DescriptionComponent : ValueElementComponent<string>
{
    public override RenderFragment GetEditor(ProjectSession session, IComponentOwner component) => builder =>
    {
        builder.OpenComponent<MarkdownWrapper>(0);
        builder.AddAttribute(1, "Value", Value);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => Value = s));
        builder.AddAttribute(3, "Placeholder", "Description");
        builder.AddAttribute(3, "Minimal", true);
        builder.AddAttribute(1, "AdditionalClasses", "sq-description");
        builder.CloseComponent();
    };
}
