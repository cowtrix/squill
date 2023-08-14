namespace Squill.Data;

public abstract class ElementBase : IElement
{
    public Guid Guid { get; set; }
    public string Name { get; set; }

    public ElementBase()
    {
        Guid = System.Guid.NewGuid();
        Name = $"Untitled {GetType().Name}";
    }

    public virtual IEnumerable<(string, string)> GetAttributes()
    {
        yield break;
    }
}
