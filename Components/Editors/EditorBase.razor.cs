using Microsoft.AspNetCore.Components;
using MudBlazor;
using Squill.Data;

namespace Squill.Components.Editors;

public partial class EditorBase<T> : ComponentBase, IAsyncDisposable
    where T: class, IElement
{
    [Parameter]
    public T Element { get; set; }

    [Parameter]
    public ProjectSession Session { get; set; }

    [Parameter]
    public TabManager TabManager { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected MudForm Form { get; set; }

    public async ValueTask DisposeAsync()
    {
        await Session.UpdateElement(Element);
    }

    protected virtual async Task Invalidate()
    {
        await Session.UpdateElement(Element);
        StateHasChanged();
    }
}
