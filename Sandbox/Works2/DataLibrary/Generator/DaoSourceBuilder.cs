using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DataLibrary.Attributes;
using DataLibrary.Engine;
using DataLibrary.Providers;
using Smart.Collections.Generic;
using Smart.ComponentModel;

namespace DataLibrary.Generator
{
    internal sealed class DaoSourceBuilder
    {
        private const string ImplementSuffix = "_Impl";

        private const string CtorArg = "engine";
        private const string EngineField = "_engine";
        private const string EngineFieldRef = "this." + EngineField;
        private const string ProviderField = "_provider";
        private const string ProviderFieldRef = "this." + ProviderField;

        private const string ConvertField = "_convert";

        private const string ConnectionVar = "_con";
        private const string CommandVar = "_cmd";

        private static readonly string EngineTypeName = GeneratorHelper.MakeGlobalName(typeof(ExecuteEngine));
        private static readonly string RuntimeHelperTypeName = GeneratorHelper.MakeGlobalName(typeof(RuntimeHelper));
        private static readonly string MethodNoAttributeTypeName = GeneratorHelper.MakeGlobalName(typeof(MethodNoAttribute));
        private static readonly string ProviderTypeName = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
        private static readonly string ConverterTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<object, object>));
        private static readonly string InSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Action<DbCommand, string, object>));
        private static readonly string InOutSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, object, DbParameter>));
        private static readonly string OutSetupTypeName = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, DbParameter>));

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

        private readonly string interfaceFullName;

        private readonly string implementName;

        private readonly bool useDefaultProvider;

        private readonly ProviderAttribute provider;

        private bool newLine;

        private int indent;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public DaoSourceBuilder(Type targetType)
        {
            this.targetType = targetType;

            interfaceFullName = GeneratorHelper.MakeGlobalName(targetType);
            implementName = targetType.Name + ImplementSuffix;
            useDefaultProvider = methods.Any(x => (x.ConnectionParameter == null) && (x.TransactionParameter == null));
            provider = targetType.GetCustomAttribute<ProviderAttribute>();
        }

        public void AddMethod(MethodMetadata mm)
        {
            methods.Add(mm);
        }

        //--------------------------------------------------------------------------------
        // Build
        //--------------------------------------------------------------------------------

        public DaoSource Build()
        {
            source.Clear();
            references.Clear();
            references.AddRange(DefaultAssemblies);
            references.Add(targetType.Assembly);
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
                BeginConnection(mm);

                DefinePreProcess(mm);

                // TODO loader and call Define

                DefinePostProcess(mm);

                EndConnection(mm);
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
            A("internal sealed class ").A(className).A(" : ").A(interfaceFullName).CR();
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

            // TODO Default
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

            // Per method
            foreach (var mm in methods)
            {
                var pos = source.Length;

                if (mm.MethodType == MethodType.ExecuteScalar)
                {
                    A("private readonly ").A(ConverterTypeName).A(" ").A(GetConvertFieldName(mm.No)).A(";").CR();
                }

                if (source.Length != pos)
                {
                    CR();
                }

                // Out converters

                // TODO

                // Setup

                // TODO

                //        public void DefineField(Type type, string memberName)
                //        {
                //            A("private readonly ").AppendType(type).A(" ").A(memberName).A(";").CR();
                //        }
            }
        }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        private void BeginConstructor()
        {
            A("public ").A(implementName).A("(").A(EngineTypeName).A(" ").A(CtorArg).A(")").CR();
            A("{").CR();
            indent++;
        }

        private void InitializeFields()
        {
            A(EngineFieldRef).A(" = ").A(CtorArg).A(";").CR();
            CR();

            if (useDefaultProvider)
            {
                A(ProviderFieldRef).A(" = ").A(CtorArg).A(".Components.Get<").A(ProviderTypeName).A(">();").CR();
            }

            // TODO other provider if need
            //foreach (var mm in methods.Where(x => x.Provider != null))
            //{

            //}

            CR();

            // Per method
            foreach (var mm in methods)
            {
                var pos = source.Length;

                if (mm.MethodType == MethodType.ExecuteScalar)
                {
                    // TODO if exist, to outer if ?
                    A($"var method{mm.No} = ").A(RuntimeHelperTypeName).A(".GetInterfaceMethodByNo(GetType(), typeof(").A(interfaceFullName).A($"), {mm.No});").CR();
                    A(GetConvertFieldNameRef(mm.No)).A(" = ").A(CtorArg).A(".CreateConverter<").A(GeneratorHelper.MakeGlobalName(mm.EngineResultType)).A($">(method{mm.No});").CR();

                    references.Add(mm.EngineResultType.Assembly);
                }

                if (source.Length != pos)
                {
                    CR();
                }

                // Out converters

                // TODO

                // Setup

                // TODO
            }
        }

        //--------------------------------------------------------------------------------
        // Method
        //--------------------------------------------------------------------------------

        private void BeginMethod(MethodMetadata mm)
        {
            A("[").A(MethodNoAttributeTypeName).A($"({mm.No})]").CR();

            A("public ");
            if (mm.IsAsync)
            {
                A("async ");
            }

            A(GeneratorHelper.MakeGlobalName(mm.MethodInfo.ReturnType)).A(" ").A(mm.MethodInfo.Name).A("(");
            references.Add(mm.MethodInfo.ReturnType.Assembly);

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
                A(GeneratorHelper.MakeGlobalName(parameterType)).A(" ").A(pi.Name);
                references.Add(parameterType.Assembly);
            }

            A(")").CR();

            A("{").CR();
            indent++;
        }

        //--------------------------------------------------------------------------------
        // Connection
        //--------------------------------------------------------------------------------

        private void BeginConnection(MethodMetadata mm)
        {
            switch (mm.MethodType)
            {
                case MethodType.Execute:
                case MethodType.ExecuteScalar:
                case MethodType.QueryFirstOrDefault:
                    if (mm.ConnectionParameter == null)
                    {
                        // TODO simple
                    }
                    else
                    {
                        // TODO simple by parameter
                    }
                    break;
                case MethodType.ExecuteReader:
                    if (mm.ConnectionParameter == null)
                    {
                        // TODO
                    }
                    else
                    {
                        // TODO
                    }
                    break;
                case MethodType.Query:
                    // TODO buffer / non buffer
                    break;
            }
        }

        private void EndConnection(MethodMetadata mm)
        {
            // TODO
            // dummy
            if (mm.MethodInfo.ReturnType != typeof(void))
            {
                A("return default;").CR();
            }

        }

        // TODO open, close ConnectionVar,CommandVar

        //--------------------------------------------------------------------------------
        // PreProcess/PostProcess
        //--------------------------------------------------------------------------------

        private void DefinePreProcess(MethodMetadata mm)
        {
            // TODO
            // dummy
            foreach (var pi in mm.MethodInfo.GetParameters())
            {
                if (pi.IsOut)
                {
                    A($"{pi.Name} = default;").CR();
                }
            }

        }

        private void DefinePostProcess(MethodMetadata mm)
        {
            // TODO
        }

        //--------------------------------------------------------------------------------
        // Block
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        // TODO
    }
}
