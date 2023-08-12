namespace Squill.Data;

public abstract class GitElement : IElement
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
}
