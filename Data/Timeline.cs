using Squill.Shared;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Squill.Data;

[ElementDisplay(Icon = "Timeline")]
public class Timeline : ElementBase
{
    public override bool ShouldTag => false;

    

    public List<Event> Events { get; set; } = new List<Event>();

    public void SetIndex(Event ev, int index)
    {
        if(index < 0 || index >= Events.Count)
        {
            return;
        }
        Events.Remove(ev);
        Events.Insert(index, ev);
    }
}

public class Event
{
    public string Title { get; set; }
    public List<EventComponent> Components { get; set; } = new List<EventComponent>();
}

public abstract class EventComponent
{
    [JsonIgnore]
    public abstract string Type { get; }
    [JsonIgnore]
    public abstract object Value { get; set; }
    public abstract RenderFragment GetEditor();
}

[ElementDisplay(Name = "Description")]
public class DescriptionEventComponent : EventComponent
{
    public override string Type => "Description";

    public override object Value { get => Description; set => Description = (string)value; }

    public string Description { get; set; }

    public override RenderFragment GetEditor() => builder =>
    {
        var callback = new EventCallback<string>();
        builder.OpenComponent<MudTextField<string>>(0);
        builder.AddAttribute(1, "Value", Description);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, (s) => Description = s));
        builder.AddAttribute(3, "Label", Type);
        builder.AddAttribute(3, "Lines", 5);
        builder.CloseComponent();
    };
}