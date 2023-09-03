namespace Squill.Data.Elements;

public interface IOwnedElement : IElement
{
    string Owner { get; set; }
}

public abstract class OwnedElement<T> : ElementBase, IOwnedElement
    where T : class, IElement
{
    public string Owner { get; set; }
}
