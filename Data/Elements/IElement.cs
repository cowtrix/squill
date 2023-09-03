﻿namespace Squill.Data.Elements;

public interface IElement
{
    Guid Guid { get; }
    IEnumerable<(string, string)> GetAttributes(ProjectSession session);
    bool ShouldTag { get; }
    string ScratchPad { get; set; }
}

public interface IContainerElement : IElement
{
    IEnumerable<Guid> Children { get; }
}