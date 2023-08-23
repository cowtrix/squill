namespace Squill.Data;

public interface IElement
{
	Guid Guid { get; }
	IEnumerable<(string, string)> GetAttributes(ProjectSession session);
	bool ShouldTag { get; }
}

public interface IContainerElement : IElement
{
	IEnumerable<Guid> Children { get; }
}