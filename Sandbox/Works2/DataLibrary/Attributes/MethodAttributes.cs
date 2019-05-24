namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAttribute : Attribute
    {
    }

    public sealed class ExecuteAttribute : MethodAttribute
    {
    }

    public sealed class ExecuteScalarAttribute : MethodAttribute
    {
    }

    // TODO ExecuteReader CommandBehavior

    // TODO FirstOrDefault ?

    public sealed class QueryAttribute : MethodAttribute
    {
    }
}
