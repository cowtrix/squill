using Squill.Shared;
using Microsoft.AspNetCore.Components;
using Squill.Data.Elements;
using Squill.Components;

namespace Squill.Data.ElementComponents;

[ElementDisplay(Name = "Element Link")]
[MultipleComponent]
public class ElementLinkComponent : ValueElementComponent<Guid>
{
    public override RenderFragment GetEditor(ProjectSession session, IComponentOwner component) => builder =>
    {
        builder.OpenComponent<ElementPicker>(0);
        builder.AddAttribute(1, "Value", Value);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<Guid>(this, (s) => Value = s));
        builder.AddAttribute(3, "ElementType", typeof(IElement));
        builder.CloseComponent();
    };
}
