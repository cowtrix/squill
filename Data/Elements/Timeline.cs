using Squill.Shared;
using Newtonsoft.Json;
using Squill.Data.ElementComponents;

namespace Squill.Data.Elements;

[ElementDisplay(Icon = "calendar-range-fill")]
public class Timeline : ElementBase
{
    public override bool ShouldTag => false;

    public List<Event> Events { get; set; } = new List<Event>();

    public void SetIndex(Event ev, int index)
    {
        if (index < 0 || index >= Events.Count)
        {
            return;
        }
        Events.Remove(ev);
        Events.Insert(index, ev);
    }
}

[ComponentType(typeof(DescriptionComponent), typeof(ElementLinkComponent))]
public class Event : ElementComponentOwnerBase
{
    public string Title { get; set; }
}
