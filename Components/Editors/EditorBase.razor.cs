using Microsoft.AspNetCore.Components;
using MudBlazor;
using Squill.Data;

namespace Squill.Components.Editors;

public abstract class EditorBase : ComponentBase, IAsyncDisposable
{
    [CascadingParameter]
    public IElement Element { get; set; }

    [CascadingParameter]
    public ProjectSession Session { get; set; }

    [CascadingParameter]
    public MudForm Form { get; set; }

    [CascadingParameter]
    public EditorWrapper Wrapper { get; set; }

    [CascadingParameter]
    public Squill.Components.TabManager TabManager { get; set; }

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

public abstract partial class GenericEditorBase<T> : EditorBase
    where T: class, IElement
{
    public T Target { get => Element as T; set => Element = value; }
}
