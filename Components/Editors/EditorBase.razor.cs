using Microsoft.AspNetCore.Components;
using Squill.Components.Layout;
using Squill.Data;
using Squill.Data.Elements;

namespace Squill.Components.Editors;

public abstract class EditorBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter]
    public IElement Element { get; set; }

    [CascadingParameter]
    public ElementMetaData Meta { get; set; }

    [CascadingParameter]
    public ProjectSession Session { get; set; }

    [CascadingParameter]
    public EditorWrapper Wrapper { get; set; }

    public virtual RenderFragment Toolbar { get; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Wrapper.CurrentEditor = this;
    }

    public virtual async ValueTask DisposeAsync()
    {
        await Session.UpdateElement(Element);
    }

    protected virtual async Task Invalidate()
    {
        await Session.UpdateElement(Element);
        StateHasChanged();
    }
}
