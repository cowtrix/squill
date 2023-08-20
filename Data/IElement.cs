namespace Squill.Data;

public interface IElement
{
	Guid Guid { get; }
	IEnumerable<(string, string)> GetAttributes();
}

public interface IContainerElement : IElement
{
	IEnumerable<Guid> Children { get; }
}