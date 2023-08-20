namespace Squill.Data;

public abstract class ElementBase : IElement
{
    public Guid Guid { get; set; }

    public ElementBase()
    {
        Guid = System.Guid.NewGuid();
    }

    public virtual IEnumerable<(string, string)> GetAttributes()
    {
        yield break;
    }
}
