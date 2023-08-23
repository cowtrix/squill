namespace Squill.Data;

public abstract class ElementBase : IElement
{
    public Guid Guid { get; set; }
    public abstract bool ShouldTag { get; }

    public ElementBase()
    {
        Guid = System.Guid.NewGuid();
    }

    public virtual IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        if (!ShouldTag)
        {
            yield return ("notag", "true");
        }
        yield break;
    }
}
