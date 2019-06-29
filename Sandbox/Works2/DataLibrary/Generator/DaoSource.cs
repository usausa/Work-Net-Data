namespace DataLibrary.Generator
{
    using System;
    using System.Reflection;

    public sealed class DaoSource
    {
        public Type TargetType { get; }

        public string ImplementTypeFullName { get; }

        public string Code { get; }

        public DaoSource(Type targetType, string implementFullName, string code)
        {
            TargetType = targetType;
            ImplementTypeFullName = implementFullName;
            Code = code;
        }
    }
}
