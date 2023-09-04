using Squill.Shared;

namespace Squill.Data.ElementComponents;


public abstract class ValueElementComponent<T> : ElementComponent
{
    public T Value { get; set; }
}
