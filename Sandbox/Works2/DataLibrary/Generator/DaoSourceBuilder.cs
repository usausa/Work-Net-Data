using System;
using System.Collections.Generic;
using System.Data;
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
        private static readonly string ConvertHelperType = GeneratorHelper.MakeGlobalName(typeof(ConvertHelper));
        private static readonly string MethodNoAttributeType = GeneratorHelper.MakeGlobalName(typeof(MethodNoAttribute));
        private static readonly string ProviderType = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
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

                switch (mm.MethodType)
                {
                    case MethodType.Execute:
                        DefineMethodExecute(mm);
                        break;
                    case MethodType.ExecuteScalar:
                        DefineMethodExecuteScalar(mm);
                        break;
                    case MethodType.ExecuteReader:
                        DefineMethodExecuteReader(mm);
                        break;
                    case MethodType.Query:
                        if (!GeneratorHelper.IsListType(mm.EngineResultType))
                        {
                            DefineMethodQueryNonBuffer(mm);
                        }
                        else
                        {
                            DefineMethodQueryBuffer(mm);
                        }
                        break;
                    case MethodType.QueryFirstOrDefault:
                        DefineMethodQueryFirstOrDefault(mm);
                        break;
                }
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
                if (mm.TimeoutParameter.ParameterType != typeof(int))
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
                case MethodType.ExecuteScalar:
                    if (!IsValidExecuteScalarResultType(mm.EngineResultType))
                    {
                        throw new AccessorException($"ReturnType is not match for MethodType.ExecuteScalar. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.Query:
                    if (!IsValidQueryResultType(mm.EngineResultType))
                    {
                        throw new AccessorException($"ReturnType is not match for MethodType.Query. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.QueryFirstOrDefault:
                    if (!IsValidQueryFirstOrDefaultResultType(mm.EngineResultType))
                    {
                        throw new AccessorException($"ReturnType is not match for MethodType.QueryFirstOrDefault. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
            }
        }

        private static bool IsValidExecuteResultType(Type type)
        {
            return type == typeof(int) || type == typeof(void);
        }

        private static bool IsValidExecuteScalarResultType(Type type)
        {
            return type != typeof(void);
        }

        // TODO reader

        private static bool IsValidQueryResultType(Type type)
        {
            return GeneratorHelper.IsEnumerableType(type) || GeneratorHelper.IsListType(type);
        }

        private static bool IsValidQueryFirstOrDefaultResultType(Type type)
        {
            return type != typeof(void);
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
                if ((mm.MethodType == MethodType.ExecuteScalar) && (mm.EngineResultType != typeof(object)))
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

                if ((mm.MethodType == MethodType.ExecuteScalar) && (mm.EngineResultType != typeof(object)))
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
        // Execute
        //--------------------------------------------------------------------------------

        private void DefineMethodExecute(MethodMetadata mm)
        {
            BeginMethod(mm);

            BeginConnectionSimple(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineConnectionOpen(mm);

            // Body
            Indent();

            if (mm.EngineResultType != typeof(void))
            {
                Append($"var {ResultVar} = ");
            }

            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteAsync({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.Execute({commandOption}{CommandVar});");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            EndConnectionSimple(mm);

            End();
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        private void DefineMethodExecuteScalar(MethodMetadata mm)
        {
            BeginMethod(mm);

            BeginConnectionSimple(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineConnectionOpen(mm);

            // Body
            Indent();
            Append($"var {ResultVar} = ");

            if (mm.EngineResultType != typeof(object))
            {
                Append($"{ConvertHelperType}.Convert<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(");
                NewLine();
                indent++;
                Indent();
            }

            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteScalarAsync({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false)");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteScalar({commandOption}{CommandVar})");
            }

            if (mm.EngineResultType != typeof(object))
            {
                Append(",");
                NewLine();
                Indent();
                Append($"{GetConvertFieldNameRef(mm.No)});");
                indent--;
            }
            else
            {
                Append(";");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            EndConnectionSimple(mm);

            End();
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        private void DefineMethodExecuteReader(MethodMetadata mm)
        {
            BeginMethod(mm);

            // TODO
            BeginConnectionSimple(mm);
            DefinePreProcess(mm);
            DefineConnectionOpen(mm);

            // TODO

            DefinePostProcess(mm);

            // TODO
            // dummy
            if (mm.MethodInfo.ReturnType != typeof(void))
            {
                AppendLine("return default;");
            }

            indent--;
            AppendLine("}");

            End();
        }

        //--------------------------------------------------------------------------------
        // QueryNonBuffer
        //--------------------------------------------------------------------------------

        private void DefineMethodQueryNonBuffer(MethodMetadata mm)
        {
            BeginMethod(mm);

            // TODO
            BeginConnectionSimple(mm);
            DefinePreProcess(mm);
            DefineConnectionOpen(mm);

            // TODO

            DefinePostProcess(mm);

            // TODO
            // dummy
            if (mm.MethodInfo.ReturnType != typeof(void))
            {
                AppendLine("return default;");
            }

            indent--;
            AppendLine("}");

            End();
        }

        //--------------------------------------------------------------------------------
        // QueryBuffer
        //--------------------------------------------------------------------------------

        private void DefineMethodQueryBuffer(MethodMetadata mm)
        {
            BeginMethod(mm);

            BeginConnectionSimple(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineConnectionOpen(mm);

            // Body
            Indent();
            Append($"var {ResultVar} = ");

            var resultType = GeneratorHelper.MakeGlobalName(GeneratorHelper.GetElementType(mm.EngineResultType));
            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.QueryBufferAsync<{resultType}>({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.QueryBuffer<{resultType}>({commandOption}{CommandVar});");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            EndConnectionSimple(mm);

            End();
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        private void DefineMethodQueryFirstOrDefault(MethodMetadata mm)
        {
            BeginMethod(mm);

            BeginConnectionSimple(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineConnectionOpen(mm);

            // Body
            Indent();
            Append($"var {ResultVar} = ");

            var resultType = GeneratorHelper.MakeGlobalName(mm.EngineResultType);
            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.QueryFirstOrDefaultAsync<{resultType}>({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.QueryFirstOrDefault<{resultType}>({commandOption}{CommandVar});");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            EndConnectionSimple(mm);

            End();
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private void BeginMethod(MethodMetadata mm)
        {
            AppendLine($"[{MethodNoAttributeType}({mm.No})]");

            Indent();
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

        private void BeginConnectionSimple(MethodMetadata mm)
        {
            if (!mm.HasConnectionParameter)
            {
                AppendLine($"using (var {ConnectionVar} = {ProviderFieldRef}.CreateConnection())");
            }

            AppendLine($"using (var {CommandVar} = {GetConnectionName(mm)}.CreateCommand())");
            AppendLine("{");
            indent++;

            DefineCommandOption(mm);
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

        // TODO *

        // TODO *

        // TODO *

        private void DefineCommandOption(MethodMetadata mm)
        {
            if (mm.CommandType != CommandType.Text)
            {
                AppendLine($"{CommandVar}.CommandType = {mm.CommandType}");
            }

            if (mm.Timeout != null)
            {
                AppendLine($"{CommandVar}.CommandTimeout = {mm.Timeout.Timeout};");
            }
            else if (mm.TimeoutParameter != null)
            {
                AppendLine($"{CommandVar}.CommandTimeout = {mm.TimeoutParameter.Name};");
            }
        }

        private void DefineConnectionOpen(MethodMetadata mm)
        {
            if (!mm.HasConnectionParameter)
            {
                if (mm.IsAsync)
                {
                    var cancelVar = mm.CancelParameter?.Name ?? string.Empty;
                    AppendLine($"await {ConnectionVar}.OpenAsync({cancelVar}).ConfigureAwait(false);");
                }
                else
                {
                    AppendLine($"{ConnectionVar}.Open();");
                }

                NewLine();
            }
        }

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
    }
}
