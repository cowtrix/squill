using Squill.Shared;
using static MudBlazor.CategoryTypes;

namespace Squill.Data;

[ElementDisplay(Icon = "Timeline")]
public class Timeline : ElementBase
{
    public override bool ShouldTag => false;

    public class Event
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Links { get; set; } = new List<string>();
    }

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
