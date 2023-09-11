using Squill.Data;

namespace Squill.Services;

public class TabService
{
    public class TabView
    {
        public Guid Id { get; set; }
        public bool Sticky { get; set; }
        public bool Closed { get; set; }
        public string? NameOverride { get; set; }
        public ElementMetaData? Target { get; set; }
        public bool IsEditing { get; set; }
    }

    public TabView CurrentTab => Tabs.ElementAtOrDefault(Index);
    public event EventHandler<TabView> OnTabAdded, OnTabRemoved, OnTabFocused;
    public int Index
    {
        get => __index;
        set
        {
            if (__index != value)
            {
                __index = value;
                OnTabFocused?.Invoke(this, CurrentTab);
            }
        }
    }
    private int __index;
    public int? NextIndex = null;
    public List<TabView> Tabs = new();

    public void RemoveTab(ElementMetaData element)
    {
        var tab = Tabs.SingleOrDefault(t => t.Target != null && t.Target.Guid == element.Guid);
        if (tab != null)
        {
            tab.Closed = true;
            Tabs.Remove(tab);
        }
        OnTabRemoved?.Invoke(this, tab);
    }

    public void AddTab(ElementMetaData? element, string nameOverride = null, bool sticky = false)
    {
        var existingIndex = Tabs.FindIndex(p => p.Target?.Guid == element.Guid);
        if (existingIndex >= 0)
        {
            NextIndex = existingIndex;
            return;
        }
        var tabView = new TabView
        {
            Target = element,
            Id = System.Guid.NewGuid(),
            NameOverride = nameOverride,
            Sticky = sticky
        };
        Tabs.Add(tabView);
        NextIndex = Tabs.Count - 1;
        OnTabAdded?.Invoke(this, tabView);
    }

    public async Task Update()
    {
        if (!NextIndex.HasValue)
        {
            return;
        }
        Index = NextIndex.Value;
        NextIndex = null;
        OnTabFocused.Invoke(this, CurrentTab);
    }
}
