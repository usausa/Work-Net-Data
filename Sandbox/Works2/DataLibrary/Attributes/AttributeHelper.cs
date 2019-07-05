namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Helpers;
    using DataLibrary.Nodes;

    public static class AttributeHelper
    {
        public static IReadOnlyList<ParameterNode> CreateParameterNodes(MethodInfo mi)
        {
            var nodes = new List<ParameterNode>();

            foreach (var pmi in mi.GetParameters().Where(ParameterHelper.IsSqlParameter))
            {
                if (ParameterHelper.IsNestedParameter(pmi))
                {
                    nodes.AddRange(pmi.ParameterType.GetProperties()
                        .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null)
                        .Select(pi => new ParameterNode(
                            $"{pmi.Name}.{pi.Name}",
                            pi.GetCustomAttribute<ParameterAttribute>()?.Name ?? pi.Name)));
                }
                else
                {
                    nodes.Add(new ParameterNode(
                        pmi.Name,
                        pmi.GetCustomAttribute<ParameterAttribute>()?.Name ?? pmi.Name));
                }
            }

            return nodes;
        }
    }
}
