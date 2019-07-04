namespace DataLibrary.Generator
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Attributes;
    using DataLibrary.Nodes;

    internal sealed class ParameterResolveVisitor : NodeVisitorBase
    {
        private readonly List<ParameterEntry> parameters = new List<ParameterEntry>();

        public IReadOnlyList<ParameterEntry> Parameters => parameters;

        private readonly MethodInfo method;

        private int index;

        public ParameterResolveVisitor(MethodInfo method)
        {
            this.method = method;
        }

        public override void Visit(ParameterNode node)
        {
            var path = node.Source.Split('.');
            if (path.Length == 1)
            {
                var pi = GetParameterInfo(path[0]);
                var type = pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
                var direction = GetParameterDirection(pi);
                var parameterType = GetParameterType(type);
                if ((parameterType != ParameterType.Simple) && (direction != ParameterDirection.Input))
                {
                    throw new AccessorGeneratorException("TODO");   // TODO
                }

                parameters.Add(new ParameterEntry(
                    node.Source,
                    index++,
                    type,
                    direction,
                    node.ParameterName,
                    parameterType));
            }
            else if (path.Length == 2)
            {
                var pi = GetParameterInfo(path[0], path[1]);
                var type = pi.PropertyType;
                var direction = GetParameterDirection(pi);
                var parameterType = GetParameterType(type);
                if ((parameterType != ParameterType.Simple) && (direction != ParameterDirection.Input))
                {
                    throw new AccessorGeneratorException("TODO");   // TODO
                }

                parameters.Add(new ParameterEntry(
                    node.Source,
                    index++,
                    type,
                    direction,
                    node.ParameterName,
                    parameterType));
            }
            else
            {
                throw new AccessorGeneratorException("TODO");   // TODO
            }
        }

        private ParameterInfo GetParameterInfo(string parameterName)
        {
            var pi = method.GetParameters().FirstOrDefault(x => x.Name == parameterName);
            if (pi == null)
            {
                throw new AccessorGeneratorException("TODO");   // TODO
            }

            return pi;
        }

        private PropertyInfo GetParameterInfo(string parameterName, string propertyName)
        {
            var pi = GetParameterInfo(parameterName).ParameterType.GetProperty(propertyName);
            if (pi == null)
            {
                throw new AccessorGeneratorException("TODO");   // TODO
            }

            return pi;
        }

        private static ParameterDirection GetParameterDirection(ParameterInfo pi)
        {
            if (pi.IsOut)
            {
                return pi.GetCustomAttribute<ReturnValueAttribute>() != null
                    ? ParameterDirection.ReturnValue
                    : ParameterDirection.Output;
            }

            if (pi.ParameterType.IsByRef)
            {
                return ParameterDirection.InputOutput;
            }

            return ParameterDirection.Input;
        }

        private static ParameterDirection GetParameterDirection(PropertyInfo pi)
        {
            var attribute = pi.GetCustomAttribute<DirectionAttribute>();
            return attribute?.Direction ?? ParameterDirection.Input;
        }

        private static ParameterType GetParameterType(Type type)
        {
            if (type.IsArray)
            {
                return ParameterType.Array;
            }

            if (GeneratorHelper.IsListParameter(type))
            {
                return ParameterType.List;
            }

            if (GeneratorHelper.IsEnumerableParameter(type))
            {
                return ParameterType.Enumerable;
            }

            return ParameterType.Simple;
        }
    }
}
