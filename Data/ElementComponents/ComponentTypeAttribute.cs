namespace Squill.Data.ElementComponents;

public class ComponentTypeAttribute : Attribute
{
    public Type[] ValidTypes { get; init; }

    public ComponentTypeAttribute(params Type[] validTypes)
    {
        ValidTypes = validTypes;
    }
}

public class DefaultComponentTypeAttribute : Attribute
{
    public Type[] DefaultTypes { get; init; }

    public DefaultComponentTypeAttribute(params Type[] validTypes)
    {
        DefaultTypes = validTypes;
    }
}

public class MultipleComponentAttribute : Attribute
{
}