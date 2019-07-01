namespace DataLibrary.Attributes
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;

    using DataLibrary.Fragments;
    using DataLibrary.Loader;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAttribute : Attribute
    {
        public CommandType CommandType { get; }

        public MethodType MethodType { get; }

        protected MethodAttribute(CommandType commandType, MethodType methodType)
        {
            CommandType = commandType;
            MethodType = methodType;
        }

        public abstract IReadOnlyList<IFragment> GetFragments(ISqlLoader loader, MethodInfo mi);
    }
}
