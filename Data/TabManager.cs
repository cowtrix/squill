using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Squill.Data;

public class TabManager
{
    public class TabView
    {
        public Guid Id { get; set; }
        public bool Sticky { get; set; }
        public string? NameOverride { get; set; }
        public ElementMetaData? Target { get; set; }

        public bool IsEditing { get; set; }
    }

    public int Index = 0;
    public int? NextIndex = null;
    public List<TabView> Tabs = new();
    public event EventHandler OnUpdated;

    public void RemoveTab(MudTabPanel tabPanel)
    {
        var tab = (TabView)tabPanel.Tag;
        if (tab != null)
        {
            Tabs.Remove(tab);
        }
    }

    public void AddTab(ElementMetaData? element, string nameOverride = null, bool sticky = false)
    {
        var tabView = new TabView { 
            Target = element, 
            Id = System.Guid.NewGuid(),
            NameOverride = nameOverride,
            Sticky = sticky
        };
        Tabs.Add(tabView);
        NextIndex = Tabs.Count - 1;
        OnUpdated?.Invoke(this, null);
    }
}
