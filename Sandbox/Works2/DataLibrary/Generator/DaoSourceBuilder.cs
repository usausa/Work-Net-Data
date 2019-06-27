using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DataLibrary.Attributes;
using DataLibrary.Engine;
using DataLibrary.Loader;
using DataLibrary.Providers;
using Smart.Collections.Generic;
using Smart.ComponentModel;

namespace DataLibrary.Generator
{
    public sealed class DaoSourceBuilder
    {
        private const string ImplementSuffix = "_Impl";

        private const string ConstructorArgument = "engine";
        private const string EngineField = "_engine";
        private const string EngineFieldRef = "this." + EngineField;
        private const string ProviderField = "_provider";
        private const string ProviderFieldRef = "this." + ProviderField;

        private static readonly string EngineTypeName = $"global::{typeof(ExecuteEngine).Namespace}.{typeof(ExecuteEngine).Name}";
        private static readonly string ProviderTypeName = $"global::{typeof(IDbProvider).Namespace}.{typeof(IDbProvider).Name}";

        // TODO Base class all ref ?
        private static readonly Assembly[] DefaultAssemblies =
        {
            Assembly.Load("System.Runtime"),
            Assembly.Load("netstandard"),
            typeof(object).Assembly,
            typeof(ExecuteEngine).Assembly,
            typeof(IComponentContainer).Assembly,
            typeof(IServiceProvider).Assembly,
        };

        private readonly Type targetType;

        private readonly List<MethodMetadata> methods = new List<MethodMetadata>();

        private readonly StringBuilder source = new StringBuilder();

        private readonly HashSet<Assembly> references = new HashSet<Assembly>();

        private readonly string implementName;

        private readonly bool useDefaultProvider;

        private bool newLine;

        private int indent;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public DaoSourceBuilder(Type targetType)
        {
            if (targetType.GetCustomAttribute<DaoAttribute>() is null)
            {
                throw new AccessorException($"Type is not supported for generation. type=[{targetType.FullName}]");
            }

            this.targetType = targetType;

            var no = 1;
            foreach (var method in targetType.GetMethods())
            {
                var attribute = method.GetCustomAttribute<MethodAttribute>(true);
                if (attribute == null)
                {
                    throw new AccessorException($"Method is not supported for generation. type=[{targetType.FullName}], method=[{method.Name}]");
                }

                methods.Add(new MethodMetadata(no, method, attribute));
                no++;
            }

            implementName = targetType.Name + ImplementSuffix;
            useDefaultProvider = methods.Any(x => (x.ConnectionParameter == null) && (x.TransactionParameter == null));
        }

        //--------------------------------------------------------------------------------
        // Build
        //--------------------------------------------------------------------------------

        public DaoSource Build(ISqlLoader loader)
        {
            source.Clear();
            references.Clear();
            references.AddRange(DefaultAssemblies);
            newLine = true;
            indent = 0;

            // Namespace
            BeginNamespace();

            // Using
            // TODO namespaces(block only ?)
            // TODO default helper

            // Class
            BeginClass(implementName);

            // Member
            DefineEngineField();
            DefineProviderField();
            // TODO provider if need, and other provider

            // Constructor
            NewLine();
            BeginConstructor();
            InitializeEngineField();
            InitializeProviderField();
            // TODO memberInit:, convert, parameter
            End();  // Constructor

            foreach (var mm in methods)
            {
                // Method
                NewLine();
                BeginMethod(mm);

                // TODO open(versions)

                // TODO pre

                // TODO loader and call

                // TODO post

                // dummy
                foreach (var pi in mm.MethodInfo.GetParameters())
                {
                    if (pi.IsOut)
                    {
                        AppendLine($"{pi.Name} = default;");
                    }
                }

                // dummy
                if (mm.MethodInfo.ReturnType != typeof(void))
                {
                    AppendLine("return default;");
                }

                // TODO close(versions)

                End();  // Method
            }

            End();  // Class
            End();  // Namespace

            return new DaoSource(
                targetType,
                $"{targetType.Namespace}.{implementName}",
                source.ToString(),
                references.OrderBy(x => x.FullName).ToArray());
        }

        //--------------------------------------------------------------------------------
        // Naming
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private void AppendIndent()
        {
            if (newLine)
            {
                for (var i = 0; i < indent; i++)
                {
                    source.Append("    ");
                }
                newLine = false;
            }
        }

        private DaoSourceBuilder Append(string code)
        {
            AppendIndent();
            source.Append(code);
            return this;
        }

        public DaoSourceBuilder AppendLine(string code)
        {
            AppendIndent();
            source.AppendLine(code);
            newLine = true;
            return this;
        }

        private void NewLine()
        {
            source.AppendLine();
            newLine = true;
        }

        private void End()
        {
            indent--;
            AppendLine("}");
        }

        private DaoSourceBuilder AppendType(Type type)
        {
            if (type == typeof(void))
            {
                source.Append("void");
                return this;
            }

            references.Add(type.Assembly);

            if (type.IsGenericType)
            {
                var index = type.Name.IndexOf('`');
                source.Append("global::").Append(type.Namespace).Append(".").Append(type.Name.Substring(0, index));
                source.Append("<");

                foreach (var argumentType in type.GetGenericArguments())
                {
                    references.Add(argumentType.Assembly);
                    AppendType(argumentType);
                }

                source.Append(">");
            }
            else
            {
                source.Append("global::").Append(type.Namespace).Append(".").Append(type.Name);
            }

            return this;
        }

        //--------------------------------------------------------------------------------
        // Class
        //--------------------------------------------------------------------------------

        private void BeginNamespace()
        {
            Append("namespace ").AppendLine(targetType.Namespace);
            AppendLine("{");
            indent++;
        }

        // TODO using

        private void BeginClass(string className)
        {
            Append("internal sealed class ").Append(className).Append(" : ").AppendType(targetType).NewLine();
            AppendLine("{");
            indent++;
        }

        //--------------------------------------------------------------------------------
        // Field
        //--------------------------------------------------------------------------------

        public void DefineEngineField() =>
            Append("private readonly ").Append(EngineTypeName).Append(" ").Append(EngineField).AppendLine(";");

        public void DefineProviderField()
        {
            if (useDefaultProvider)
            {
                Append("private readonly ").Append(ProviderTypeName).Append(" ").Append(ProviderField).AppendLine(";");
            }
        }

        // TODO
        //        public void DefineField(Type type, string memberName)
        //        {
        //            Append("private readonly ").AppendType(type).Append(" ").Append(memberName).AppendLine(";");
        //        }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        private void BeginConstructor()
        {
            Append("public ").Append(implementName).Append("(").Append(EngineTypeName).Append(" ").Append(ConstructorArgument).AppendLine(")");
            AppendLine("{");
            indent++;
        }

        private void InitializeEngineField() =>
            Append(EngineFieldRef).Append(" = ").Append(ConstructorArgument).AppendLine(";");

        private void InitializeProviderField()
        {
            if (useDefaultProvider)
            {
                Append(ProviderFieldRef).Append(" = ").Append(ConstructorArgument).Append(".Components.Get<").Append(ProviderTypeName).AppendLine(">();");
            }
        }

        // TODO

        //--------------------------------------------------------------------------------
        // Method
        //--------------------------------------------------------------------------------

        private void BeginMethod(MethodMetadata mm)
        {
            Append("public ");
            if (mm.IsAsync)
            {
                Append("async ");
            }

            AppendType(mm.MethodInfo.ReturnType).Append(" ").Append(mm.MethodInfo.Name).Append("(");

            var first = true;
            foreach (var pi in mm.MethodInfo.GetParameters())
            {
                if (!first)
                {
                    Append(", ");
                }
                else
                {
                    first = false;
                }

                if (pi.IsOut)
                {
                    Append("out ");
                }
                else if (pi.ParameterType.IsByRef)
                {
                    Append("ref ");
                }

                var parameterType = pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
                AppendType(parameterType).Append(" ").Append(pi.Name);
            }

            AppendLine(")");

            AppendLine("{");
            indent++;
        }

        //--------------------------------------------------------------------------------
        // Connection
        //--------------------------------------------------------------------------------

        // TODO
        //        public void BeginConnection(MethodMetadata mm)
        //        {
        //            switch (mm.Method.MethodType)
        //            {
        //                case MethodType.Execute:
        //                case MethodType.ExecuteScalar:
        //                case MethodType.QueryFirstOrDefault:
        //                    if (mm.ConnectionParameter == null)
        //                    {

        //                    }
        //                    else
        //                    {

        //                    }
        //                    break;
        //                case MethodType.ExecuteReader:
        //                    if (mm.ConnectionParameter == null)
        //                    {

        //                    }
        //                    else
        //                    {

        //                    }
        //                    break;
        //                case MethodType.Query:
        //                    // TODO buffer / non buffer
        //                    break;
        //            }
        //        }

        //--------------------------------------------------------------------------------
        // PreProcess
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // Block
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // PostProcess
        //--------------------------------------------------------------------------------

        // TODO
    }
}
