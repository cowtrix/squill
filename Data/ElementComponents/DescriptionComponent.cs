using Squill.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Squill.Data.Elements;

namespace Squill.Data.ElementComponents;

[ElementDisplay(Name = "Description")]
public class DescriptionComponent : ValueElementComponent<string>
{
    public override RenderFragment GetEditor(ProjectSession session, IComponentOwner component) => builder =>
    {
        builder.OpenComponent<MudTextField<string>>(0);
        builder.AddAttribute(1, "Value", Value);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => Value = s));
        builder.AddAttribute(3, "Label", GetType().GetName());
        builder.AddAttribute(3, "Lines", 2);
        builder.CloseComponent();
    };
}
