using System;
using System.Collections.Generic;
using System.Data.Common;
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

        private const string CtorArgName = "engine";
        private const string EngineField = "_engine";
        private const string EngineFieldRef = "this." + EngineField;
        private const string ProviderField = "_provider";
        private const string ProviderFieldRef = "this." + ProviderField;

        private const string ConvertField = "_convert";

        private static readonly string EngineTypeName = GeneratorHelper.MakeGlobalName(typeof(ExecuteEngine));
        private static readonly string ProviderTypeName = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
        private static readonly string ConverterTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<object, object>));
        private static readonly string InSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Action<DbCommand, string, object>));
        private static readonly string InOutSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, object, DbParameter>));
        private static readonly string OutSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, DbParameter>));

        // TODO Base class all ref ?
        private static readonly Assembly[] DefaultAssemblies =
        {
            Assembly.Load("System.Runtime"),
            Assembly.Load("netstandard"),
            typeof(object).Assembly,
            typeof(ExecuteEngine).Assembly,
            typeof(IComponentContainer).Assembly,
            typeof(IServiceProvider).Assembly,
            typeof(DbConnection).Assembly
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
            // TODO targetType assembly?
            newLine = true;
            indent = 0;

            // Namespace
            BeginNamespace();

            // Using
            // TODO default helper
            // TODO namespaces(static helper only ?)

            // Class
            BeginClass(implementName);

            // Member
            DefineFields();

            // Constructor
            BeginConstructor();
            InitializeFields();
            End();  // Constructor

            foreach (var mm in methods)
            {
                CR();

                // Method
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
                        A($"{pi.Name} = default;").CR();
                    }
                }

                // dummy
                if (mm.MethodInfo.ReturnType != typeof(void))
                {
                    A("return default;").CR();
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

        public string GetConvertFieldName(int no) => ConvertField + no;

        public string GetConvertFieldNameRef(int no) => "this." + GetConvertFieldName(no);

        // TODO

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private void CR()
        {
            source.AppendLine();
            newLine = true;
        }

        private DaoSourceBuilder A(string code)
        {
            if (newLine)
            {
                for (var i = 0; i < indent; i++)
                {
                    source.Append("    ");
                }
                newLine = false;
            }
            source.Append(code);
            return this;
        }


        private void End()
        {
            indent--;
            A("}").CR();
        }

        private DaoSourceBuilder AppendType(Type type)
        {
            // TODO Helper version or remove and add reference by call by?
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
            A("namespace ").A(targetType.Namespace).CR();
            A("{").CR();
            indent++;
        }

        // TODO using

        private void BeginClass(string className)
        {
            A("internal sealed class ").A(className).A(" : ").AppendType(targetType).CR();
            A("{").CR();
            indent++;
        }

        //--------------------------------------------------------------------------------
        // Field
        //--------------------------------------------------------------------------------

        private void DefineFields()
        {
            // Engine
            A("private readonly ").A(EngineTypeName).A(" ").A(EngineField).A(";").CR();
            CR();

            // Provider
            if (useDefaultProvider)
            {
                A("private readonly ").A(ProviderTypeName).A(" ").A(ProviderField).A(";").CR();
            }

            // TODO other provider if need
            //foreach (var mm in methods.Where(x => x.Provider != null))
            //{

            //}

            CR();

            // Scalar converters
            //var scalarMethods = methods.Where(x => x.Method.MethodType == MethodType.ExecuteScalar).ToList();
            //if (scalarMethods.Count > 0)
            //{
            //    foreach (var mm in scalarMethods)
            //    {
            //        A("private readonly ").A(ConverterTypeName).A(" ").A(GetConvertFieldName(mm.No)).A(";").CR();
            //    }

            //    CR();
            //}

            // Out converters

            // TODO

            // Setup

            // TODO

            //        public void DefineField(Type type, string memberName)
            //        {
            //            A("private readonly ").AppendType(type).A(" ").A(memberName).A(";").CR();
            //        }

            CR();
        }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        private void BeginConstructor()
        {
            A("public ").A(implementName).A("(").A(EngineTypeName).A(" ").A(CtorArgName).A(")").CR();
            A("{").CR();
            indent++;
        }

        private void InitializeFields()
        {
            A(EngineFieldRef).A(" = ").A(CtorArgName).A(";").CR();
            CR();

            if (useDefaultProvider)
            {
                A(ProviderFieldRef).A(" = ").A(CtorArgName).A(".Components.Get<").A(ProviderTypeName).A(">();").CR();
            }

            // TODO other provider if need
            //foreach (var mm in methods.Where(x => x.Provider != null))
            //{

            //}

            //var scalarMethods = methods.Where(x => x.Method.MethodType == MethodType.ExecuteScalar).ToList();
            //if (scalarMethods.Count > 0)
            //{
            //    foreach (var mm in scalarMethods)
            //    {
            //        //        // TODO method reference code
            //        A(GetConvertFieldNameRef(mm.No)).A(" = ").A(CtorArgName).A(".CreateConverter<");
            //        AppendType(mm.EngineResultType);
            //        A(">(method5.ReturnParameter);").CR();
            //    }

            //    CR();
            //}

            // Out converters

            // TODO

            // Setup

            // TODO
        }

        //--------------------------------------------------------------------------------
        // Method
        //--------------------------------------------------------------------------------

        private void BeginMethod(MethodMetadata mm)
        {
            A("public ");
            if (mm.IsAsync)
            {
                A("async ");
            }

            AppendType(mm.MethodInfo.ReturnType).A(" ").A(mm.MethodInfo.Name).A("(");

            var first = true;
            foreach (var pi in mm.MethodInfo.GetParameters())
            {
                if (!first)
                {
                    A(", ");
                }
                else
                {
                    first = false;
                }

                if (pi.IsOut)
                {
                    A("out ");
                }
                else if (pi.ParameterType.IsByRef)
                {
                    A("ref ");
                }

                var parameterType = pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
                AppendType(parameterType).A(" ").A(pi.Name);
            }

            A(")").CR();

            A("{").CR();
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
