using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DataLibrary.Attributes;
using DataLibrary.Engine;
using DataLibrary.Providers;

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
        private const string ResultVar = "_result";

        private static readonly string EngineType = GeneratorHelper.MakeGlobalName(typeof(ExecuteEngine));
        private static readonly string RuntimeHelperType = GeneratorHelper.MakeGlobalName(typeof(RuntimeHelper));
        private static readonly string MethodNoAttributeType = GeneratorHelper.MakeGlobalName(typeof(MethodNoAttribute));
        private static readonly string ProviderType = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
        private static readonly string ProviderAttributeType = GeneratorHelper.MakeGlobalName(typeof(ProviderAttribute));
        private static readonly string ConverterType = GeneratorHelper.MakeGlobalName(typeof(Func<object, object>));
        private static readonly string InSetupType = GeneratorHelper.MakeGlobalName(typeof(Action<DbCommand, string, object>));
        private static readonly string InOutSetupType = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, object, DbParameter>));
        private static readonly string OutSetupType = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, DbParameter>));

        private readonly Type targetType;

        private readonly List<MethodMetadata> methods = new List<MethodMetadata>();

        private readonly StringBuilder source = new StringBuilder();

        private readonly string interfaceFullName;

        private readonly string implementName;

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
                ValidateMethod(mm);

                NewLine();

                // Method
                BeginMethod(mm);
                BeginConnection(mm);

                DefinePreProcess(mm);

                // TODO block
                DefineCall(mm);

                DefinePostProcess(mm);

                EndConnection(mm);
                End();  // Method
            }

            End();  // Class
            End();  // Namespace

            return new DaoSource(
                targetType,
                $"{targetType.Namespace}.{implementName}",
                source.ToString());
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

        private void Indent()
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

        private void AppendLine(string code)
        {
            Indent();
            source.AppendLine(code);
            newLine = true;
        }

        private void Append(string code)
        {
            Indent();
            source.Append(code);
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

        //--------------------------------------------------------------------------------
        // Validate
        //--------------------------------------------------------------------------------

        private void ValidateMethod(MethodMetadata mm)
        {
            if (mm.TimeoutParameter != null)
            {
                if ((mm.TimeoutParameter.ParameterType != typeof(int)) && (mm.TimeoutParameter.ParameterType != typeof(int?)))
                {
                    throw new AccessorException($"Timeout parameter type must be int. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], parameter=[{mm.TimeoutParameter.Name}]");
                }
            }

            switch (mm.MethodType)
            {
                case MethodType.Execute:
                    if (!IsValidExecuteResultType(mm.EngineResultType))
                    {
                        throw new AccessorException($"ReturnType is not match for MethodType.Execute. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.Query:
                    if (!IsValidQueryResultType(mm.EngineResultType))
                    {
                        throw new AccessorException($"ReturnType is not match for MethodType.Query. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
            }

        }

        private static bool IsValidExecuteResultType(Type type)
        {
            return type == typeof(int) || type == typeof(void);
        }

        private static bool IsValidQueryResultType(Type type)
        {
            return type != typeof(string) &&
                   type.GetInterfaces().Prepend(type).Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private static string GetConnectionName(MethodMetadata mm)
        {
            if (mm.ConnectionParameter != null)
            {
                return mm.ConnectionParameter.Name;
            }

            if (mm.TransactionParameter != null)
            {
                return $"{mm.TransactionParameter.Name}.Connection";
            }

            return ConnectionVar;
        }

        //--------------------------------------------------------------------------------
        // Class
        //--------------------------------------------------------------------------------

        private void BeginNamespace()
        {
            AppendLine($"namespace {targetType.Namespace}");
            AppendLine("{");
            indent++;
        }

        // TODO using

        private void BeginClass(string className)
        {
            AppendLine($"internal sealed class {className} : {interfaceFullName}");
            AppendLine("{");
            indent++;
        }

        //--------------------------------------------------------------------------------
        // Field
        //--------------------------------------------------------------------------------

        private void DefineFields()
        {
            // Engine
            AppendLine($"private readonly {EngineType} {EngineField};");
            NewLine();

            // Provider
            var useDefaultProvider = methods.Any(x => (x.ConnectionParameter == null) && (x.TransactionParameter == null));
            if (useDefaultProvider)
            {
                AppendLine($"private readonly {ProviderType} {ProviderField};");
            }

            // TODO other provider if need
            //foreach (var mm in methods.Where(x => x.Provider != null))
            //{

            //}

            NewLine();

            // Per method
            foreach (var mm in methods)
            {
                var pos = source.Length;

                // TODO ignore object
                if (mm.MethodType == MethodType.ExecuteScalar)
                {
                    AppendLine($"private readonly {ConverterType} {GetConvertFieldName(mm.No)};");
                }

                if (source.Length != pos)
                {
                    NewLine();
                }

                // Out converters

                // TODO

                // Setup

                // TODO

                //        public void DefineField(Type type, string memberName)
                //        {
                //            Append("private readonly ").AppendType(type).Append(" ").Append(memberName).Append(";");NewLine();
                //        }
            }
        }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        private void BeginConstructor()
        {
            AppendLine($"public {implementName}({EngineType} {CtorArg})");
            AppendLine("{");
            indent++;
        }

        private void InitializeFields()
        {
            AppendLine($"{EngineFieldRef} = {CtorArg};");
            NewLine();

            var useDefaultProvider = methods.Any(x => (x.ConnectionParameter == null) && (x.TransactionParameter == null));
            if (useDefaultProvider)
            {
                if (provider == null)
                {
                    AppendLine($"{ProviderFieldRef} = {CtorArg}.Components.Get<{ProviderType}>();");
                }
                else
                {
                    if (!typeof(IDbProviderSelector).IsAssignableFrom(provider.SelectorType))
                    {
                        throw new AccessorException($"Provider attribute parameter is invalid. type=[{targetType.FullName}]");
                    }

                    AppendLine($"{ProviderFieldRef} = {RuntimeHelperType}.GetDbProvider({CtorArg}, typeof({interfaceFullName}));");
                }
            }

            // TODO other provider if need
            //foreach (var mm in methods.Where(x => x.Provider != null))
            //{

            //}

            NewLine();

            // Per method
            foreach (var mm in methods)
            {
                var pos = source.Length;

                if (mm.MethodType == MethodType.ExecuteScalar)
                {
                    // TODO if exist, to outer if ?
                    AppendLine($"var method{mm.No} = {RuntimeHelperType}.GetInterfaceMethodByNo(GetType(), typeof({interfaceFullName}), {mm.No});");
                    // TODO ignore object
                    AppendLine($"{GetConvertFieldNameRef(mm.No)} = {CtorArg}.CreateConverter<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(method{mm.No});");
                }

                if (source.Length != pos)
                {
                    NewLine();
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
            AppendLine($"[{MethodNoAttributeType}({mm.No})]");

            Append("public ");
            if (mm.IsAsync)
            {
                Append("async ");
            }

            Append($"{GeneratorHelper.MakeGlobalName(mm.MethodInfo.ReturnType)} {mm.MethodInfo.Name}(");

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
                Append($"{GeneratorHelper.MakeGlobalName(parameterType)} {pi.Name}");
            }

            AppendLine(")");

            AppendLine("{");
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
                    BeginConnectionSimple(mm);
                    break;
                case MethodType.ExecuteReader:
                    if (mm.ConnectionParameter == null)
                    {
                        // TODO
                        BeginConnectionSimple(mm);
                    }
                    else
                    {
                        // TODO
                        BeginConnectionSimple(mm);
                    }
                    break;
                case MethodType.Query:
                    // TODO buffer / non buffer
                    BeginConnectionSimple(mm);
                    break;
            }
        }

        private void EndConnection(MethodMetadata mm)
        {
            switch (mm.MethodType)
            {
                case MethodType.Execute:
                case MethodType.ExecuteScalar:
                case MethodType.QueryFirstOrDefault:
                    // TODO
                    EndConnectionSimple(mm);
                    break;
                default:
                    // TODO
                    // dummy
                    if (mm.MethodInfo.ReturnType != typeof(void))
                    {
                        AppendLine("return default;");
                    }

                    indent--;
                    AppendLine("}");
                    break;
            }
        }

        // Simple

        private void BeginConnectionSimple(MethodMetadata mm)
        {
            if (!mm.HasConnectionParameter)
            {
                AppendLine($"using (var {ConnectionVar} = {ProviderFieldRef}.CreateConnection())");
            }

            AppendLine($"using (var {CommandVar} = {GetConnectionName(mm)}.CreateCommand())");
            AppendLine("{");
            indent++;
        }

        private void EndConnectionSimple(MethodMetadata mm)
        {
            if (mm.EngineResultType != typeof(void))
            {
                NewLine();
                AppendLine($"return {ResultVar};");
            }

            indent--;
            AppendLine("}");
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
                    AppendLine($"{pi.Name} = default;");
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

        private void DefineCall(MethodMetadata mm)
        {
            // TODO
            switch (mm.MethodType)
            {
                case MethodType.Execute:
                    DefineCallExecute(mm);
                    break;
                case MethodType.ExecuteScalar:
                    DefineCallExecuteScalar(mm);
                    break;
                case MethodType.ExecuteReader:
                    break;
                case MethodType.Query:
                    break;
                case MethodType.QueryFirstOrDefault:
                    DefineCallQueryFirstOrDefault(mm);
                    break;
            }
        }

        private void DefineCallExecute(MethodMetadata mm)
        {
            // TODO async, con/tx
            if (mm.EngineResultType == typeof(void))
            {
                AppendLine($"{CommandVar}.ExecuteNonQuery();");
            }
            else
            {
                AppendLine($"var {ResultVar} = {CommandVar}.ExecuteNonQuery();");
            }
        }

        private void DefineCallExecuteScalar(MethodMetadata mm)
        {
            // TODO async, con/tx
            if (mm.EngineResultType == typeof(object))
            {
                AppendLine($"var {ResultVar} = {EngineFieldRef}.ExecuteScalar({CommandVar});");
            }
            else
            {
                AppendLine($"var {ResultVar} = {CommandVar}.ExecuteNonQuery();");
            }
        }

        private void DefineCallQueryFirstOrDefault(MethodMetadata mm)
        {
            // TODO async, con/tx
            var resultType = GeneratorHelper.MakeGlobalName(mm.EngineResultType);
            AppendLine($"var {ResultVar} = {EngineFieldRef}.QueryFirstOrDefault<{resultType}>({CommandVar});");
        }
    }
}
