using Squill.Data.Elements;

namespace Squill.Components.Editors;

public abstract partial class GenericEditorBase<T> : EditorBase
    where T: class, IElement
{
    public T Target { get => Element as T; set => Element = value; }
}
