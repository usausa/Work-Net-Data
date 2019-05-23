namespace DataLibrary.Attributes
{
    using System;
    using System.Data;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class DbTypeAttribute : Attribute
    {
        public DbType DbType { get; }

        public DbTypeAttribute(DbType dbType)
        {
            DbType = dbType;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class SizeAttribute : Attribute
    {
        public int Size { get; }

        public SizeAttribute(int size)
        {
            Size = size;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class TypeHandlerAttribute : Attribute
    {
        public Type Type { get; }

        public TypeHandlerAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DirectionAttribute : Attribute
    {
        public ParameterDirection Direction { get; }

        public DirectionAttribute(ParameterDirection direction)
        {
            Direction = direction;
        }
    }
}
