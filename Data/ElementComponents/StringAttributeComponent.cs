using Squill.Shared;
using Microsoft.AspNetCore.Components;
using Squill.Data.Elements;
using Squill.Components.Layout;

namespace Squill.Data.ElementComponents;

[ElementDisplay(Name = "String Attribute")]
[MultipleComponent]
public class StringAttributeComponent : ValueElementComponent<string>
{
    public string Key { get; set; }
    public override RenderFragment GetEditor(ProjectSession session, IComponentOwner component) => builder =>
    {
        var index = 0;
        builder.OpenElement(index++, "div");
        builder.AddAttribute(index++, "class", "string-attribute");

        builder.OpenComponent<SqInputText>(index++);
        builder.AddAttribute(index++, "Label", "Key");
        builder.AddAttribute(index++, "Value", Key);
        builder.AddAttribute(index++, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => Key = s));
        builder.CloseComponent();

        builder.OpenComponent<SqInputText>(index++);
        builder.AddAttribute(index++, "Value", Value);
        builder.AddAttribute(index++, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => Value = s));
        builder.CloseComponent();

        builder.CloseElement();
    };
}