namespace Smart.Data.Accessor.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Helpers;
    using Smart.Data.Accessor.Nodes;

    public static class BuildHelper
    {
        //--------------------------------------------------------------------------------
        // Table
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public static string GetTableName(MethodInfo mi, IGeneratorOption option)
        {
            var parameter = mi.GetParameters()
                .FirstOrDefault(x => ParameterHelper.IsSqlParameter(x) && ParameterHelper.IsNestedParameter(x));
            if (parameter == null)
            {
                return null;
            }

            var attr = parameter.ParameterType.GetCustomAttribute<NameAttribute>();
            if (attr != null)
            {
                return attr.Name;
            }

            var suffix = option.GetValueAsStringArray("EntityClassSuffix");
            var match = suffix.FirstOrDefault(x => parameter.ParameterType.Name.EndsWith(x));
            return match == null
                ? parameter.ParameterType.Name
                : parameter.ParameterType.Name.Substring(0, parameter.ParameterType.Name.Length - match.Length);
        }

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public static IReadOnlyList<BuildParameterInfo> GetParameters(MethodInfo mi)
        {
            var parameters = new List<BuildParameterInfo>();

            foreach (var pmi in mi.GetParameters().Where(ParameterHelper.IsSqlParameter))
            {
                if (ParameterHelper.IsNestedParameter(pmi))
                {
                    parameters.AddRange(pmi.ParameterType.GetProperties()
                        .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null)
                        .Select(pi => new BuildParameterInfo(
                            pi,
                            pi.Name,
                            pi.GetCustomAttribute<NameAttribute>()?.Name ?? pi.Name)));
                }
                else
                {
                    parameters.Add(new BuildParameterInfo(
                        pmi,
                        pmi.Name,
                        pmi.GetCustomAttribute<NameAttribute>()?.Name ?? pmi.Name));
                }
            }

            return parameters;
        }
    }
}
