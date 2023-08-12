namespace Squill.Data;

public interface IElement
{
	Guid Guid { get; }
	string Name { get; }
}

public interface IContainerElement : IElement
{
	IEnumerable<Guid> Children { get; }
}