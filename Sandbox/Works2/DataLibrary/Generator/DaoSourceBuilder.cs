using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DataLibrary.Attributes;
using DataLibrary.Engine;
using DataLibrary.Helpers;
using DataLibrary.Providers;
using DataLibrary.Scripts;

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
        private const string SetupField = "_setup";

        private const string ConnectionVar = "_con";
        private const string CommandVar = "_cmd";
        private const string ReaderVar = "_reader";
        private const string ResultVar = "_result";
        private const string WasClosedVar = "_wasClosed";
        private const string OutParamVar = "_outParam";

        private static readonly string EngineType = GeneratorHelper.MakeGlobalName(typeof(ExecuteEngine));
        private static readonly string RuntimeHelperType = GeneratorHelper.MakeGlobalName(typeof(RuntimeHelper));
        private static readonly string MethodNoAttributeType = GeneratorHelper.MakeGlobalName(typeof(MethodNoAttribute));
        private static readonly string ProviderType = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
        private static readonly string DataReaderType = GeneratorHelper.MakeGlobalName(typeof(IDataReader));
        private static readonly string DbCommandType = GeneratorHelper.MakeGlobalName(typeof(DbCommand));
        private static readonly string DbParameterType = GeneratorHelper.MakeGlobalName(typeof(DbParameter));
        private static readonly string CommandTypeType = GeneratorHelper.MakeGlobalName(typeof(CommandType));
        private static readonly string ConnectionStateType = GeneratorHelper.MakeGlobalName(typeof(ConnectionState));
        private static readonly string WrappedReaderType = GeneratorHelper.MakeGlobalName(typeof(WrappedReader));
        private static readonly string ExceptionType = GeneratorHelper.MakeGlobalName(typeof(Exception));
        private static readonly string ConverterType = GeneratorHelper.MakeGlobalName(typeof(Func<object, object>));
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
            DefineUsing();

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
                        if (!TypeHelper.IsList(mm.EngineResultType))
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

        public string GetProviderFieldName(int no) => ProviderField + no;

        public string GetProviderFieldNameRef(int no) => "this." + GetProviderFieldName(no);

        public string GetConvertFieldName(int no) => ConvertField + no;

        public string GetConvertFieldNameRef(int no) => "this." + GetConvertFieldName(no);

        public string GetConvertFieldName(int no, int index) => ConvertField + no + "_" + index;

        public string GetConvertFieldNameRef(int no, int index) => "this." + GetConvertFieldName(no, index);

        public string GetSetupFieldName(int no, int index) => SetupField + no + "_" + index;

        public string GetSetupFieldNameRef(int no, int index) => "this." + GetSetupFieldName(no, index);

        public string GetOutParamName(int index) => OutParamVar + index;

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
                    throw new AccessorGeneratorException($"Timeout parameter type must be int. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], parameter=[{mm.TimeoutParameter.Name}]");
                }
            }

            switch (mm.MethodType)
            {
                case MethodType.Execute:
                    if (!IsValidExecuteResultType(mm.EngineResultType))
                    {
                        throw new AccessorGeneratorException($"ReturnType is not match for MethodType.Execute. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.ExecuteScalar:
                    if (!IsValidExecuteScalarResultType(mm.EngineResultType))
                    {
                        throw new AccessorGeneratorException($"ReturnType is not match for MethodType.ExecuteScalar. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.ExecuteReader:
                    if (!IsValidExecuteReaderResultType(mm.EngineResultType))
                    {
                        throw new AccessorGeneratorException($"ReturnType is not match for MethodType.ExecuteReader. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.Query:
                    if (!IsValidQueryResultType(mm.EngineResultType))
                    {
                        throw new AccessorGeneratorException($"ReturnType is not match for MethodType.Query. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
                    }
                    break;
                case MethodType.QueryFirstOrDefault:
                    if (!IsValidQueryFirstOrDefaultResultType(mm.EngineResultType))
                    {
                        throw new AccessorGeneratorException($"ReturnType is not match for MethodType.QueryFirstOrDefault. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}], returnType=[{mm.MethodInfo.ReturnType}]");
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

        private static bool IsValidExecuteReaderResultType(Type type)
        {
            return type.IsAssignableFrom(typeof(DbDataReader));
        }

        private static bool IsValidQueryResultType(Type type)
        {
            return TypeHelper.IsEnumerable(type) || TypeHelper.IsList(type);
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

        private void DefineUsing()
        {
            var visitor = new UsingResolveVisitor();
            foreach (var mm in methods)
            {
                visitor.Visit(mm.Nodes);
            }

            foreach (var name in visitor.Usings)
            {
                AppendLine($"using {name};");
            }

            foreach (var name in visitor.Helpers)
            {
                AppendLine($"using static {name};");
            }

            AppendLine($"using static {typeof(ScriptHelper).Namespace}.{typeof(ScriptHelper).Name};");

            NewLine();
        }

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

            NewLine();

            // Per method
            foreach (var mm in methods)
            {
                var previous = source.Length;

                if (mm.Provider != null)
                {
                    AppendLine($"private readonly {ProviderType} {GetProviderFieldName(mm.No)};");
                }

                if ((mm.MethodType == MethodType.ExecuteScalar) &&
                    (mm.EngineResultType != typeof(object)))
                {
                    AppendLine($"private readonly {ConverterType} {GetConvertFieldName(mm.No)};");
                }

                foreach (var parameter in mm.Parameters)
                {
                    switch (parameter.Direction)
                    {
                        case ParameterDirection.Output:
                        case ParameterDirection.ReturnValue:
                            AppendLine($"private readonly {OutSetupType} {GetSetupFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterDirection.InputOutput:
                            AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeInOutParameterType(parameter.Type))} {GetSetupFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterDirection.Input:
                            switch (parameter.ParameterType)
                            {
                                case ParameterType.Array:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeArrayParameterType(parameter.Type))} {GetSetupFieldName(mm.No, parameter.Index)};");
                                    break;
                                case ParameterType.List:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeListParameterType(parameter.Type))} {GetSetupFieldName(mm.No, parameter.Index)};");
                                    break;
                                case ParameterType.Enumerable:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeEnumerableParameterType(parameter.Type))} {GetSetupFieldName(mm.No, parameter.Index)};");
                                    break;
                                default:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeInParameterType(parameter.Type))} {GetSetupFieldName(mm.No, parameter.Index)};");
                                    break;
                            }
                            break;
                    }
                }

                foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input && x.Type != typeof(object)))
                {
                    AppendLine($"private readonly {ConverterType} {GetConvertFieldName(mm.No, parameter.Index)};");
                }

                if (source.Length > previous)
                {
                    NewLine();
                }
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
                        throw new AccessorGeneratorException($"Provider attribute parameter is invalid. type=[{targetType.FullName}]");
                    }

                    AppendLine($"{ProviderFieldRef} = {RuntimeHelperType}.GetDbProvider({CtorArg}, typeof({interfaceFullName}));");
                }
            }

            // Per method
            foreach (var mm in methods)
            {
                var hasProvider = mm.Provider != null;
                var hasConverter = (mm.MethodType == MethodType.ExecuteScalar) &&
                                   (mm.EngineResultType != typeof(object));

                if (hasProvider || hasConverter || mm.Parameters.Count > 0)
                {
                    NewLine();
                    AppendLine($"var method{mm.No} = {RuntimeHelperType}.GetInterfaceMethodByNo(GetType(), typeof({interfaceFullName}), {mm.No});");

                    if (hasProvider)
                    {
                        if (!typeof(IDbProviderSelector).IsAssignableFrom(mm.Provider.SelectorType))
                        {
                            throw new AccessorGeneratorException($"Provider attribute parameter is invalid. type=[{targetType.FullName}], method=[{mm.MethodInfo.Name}]");
                        }

                        AppendLine($"{GetProviderFieldNameRef(mm.No)} = {RuntimeHelperType}.GetDbProvider({CtorArg}, method{mm.No});");
                    }

                    if (hasConverter)
                    {
                        AppendLine($"{GetConvertFieldNameRef(mm.No)} = {CtorArg}.CreateConverter<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(method{mm.No});");
                    }

                    foreach (var parameter in mm.Parameters)
                    {
                        Indent();
                        Append($"{GetSetupFieldNameRef(mm.No, parameter.Index)} = ");


                        switch (parameter.Direction)
                        {
                            case ParameterDirection.Output:
                                Append($"{CtorArg}.CreateReturnParameterSetup();");
                                break;
                            case ParameterDirection.ReturnValue:
                                Append($"{RuntimeHelperType}.GetOutParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                break;
                            case ParameterDirection.InputOutput:
                                Append($"{RuntimeHelperType}.GetInOutParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                break;
                            case ParameterDirection.Input:
                                switch (parameter.ParameterType)
                                {
                                    case ParameterType.Array:
                                        Append($"{RuntimeHelperType}.GetArrayParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type.GetElementType())}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                    case ParameterType.List:
                                        Append($"{RuntimeHelperType}.GetListParameterSetup<{GeneratorHelper.MakeGlobalName(TypeHelper.GetListElementType(parameter.Type))}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                    case ParameterType.Enumerable:
                                        Append($"{RuntimeHelperType}.GetEnumerableParameterSetup<{GeneratorHelper.MakeGlobalName(TypeHelper.GetEnumerableElementType(parameter.Type))}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                    default:
                                        Append($"{RuntimeHelperType}.GetInParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                }
                                break;
                        }

                        NewLine();
                    }

                    foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input && x.Type != typeof(object)))
                    {
                        AppendLine($"{GetConvertFieldNameRef(mm.No, parameter.Index)} = {RuntimeHelperType}.GetConverter<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                    }
                }
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

            DefineSql(mm);

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

            DefineSql(mm);

            DefineConnectionOpen(mm);

            // Execute
            Indent();
            Append($"var {ResultVar} = ");

            if (mm.EngineResultType != typeof(object))
            {
                Append($"{RuntimeHelperType}.Convert<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(");
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

            BeginConnectionForReader(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineSql(mm);

            DefineConnectionOpen(mm);

            // Execute
            Indent();
            Append($"var {ResultVar} = ");

            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, {WasClosedVar}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteReaderAsync({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteReader({commandOption}{CommandVar});");
            }

            NewLine();

            if (mm.HasConnectionParameter)
            {
                AppendLine($"{WasClosedVar} = false;");
            }

            // PostProcess
            DefinePostProcess(mm);

            NewLine();

            AppendLine($"return new {WrappedReaderType}({CommandVar}, {ReaderVar});");
            EndConnectionForReader(mm);

            End();
        }

        //--------------------------------------------------------------------------------
        // QueryNonBuffer
        //--------------------------------------------------------------------------------

        private void DefineMethodQueryNonBuffer(MethodMetadata mm)
        {
            BeginMethod(mm);

            BeginConnectionForReader(mm);

            // PreProcess
            DefinePreProcess(mm);

            DefineSql(mm);

            DefineConnectionOpen(mm);

            // Body
            Indent();
            Append($"var {ResultVar} = ");

            var commandOption = mm.HasConnectionParameter ? $"{GetConnectionName(mm)}, {WasClosedVar}, " : string.Empty;
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteReaderAsync({commandOption}{CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteReader({commandOption}{CommandVar});");
            }

            NewLine();

            if (mm.HasConnectionParameter)
            {
                AppendLine($"{WasClosedVar} = false;");
            }

            // PostProcess
            DefinePostProcess(mm);

            NewLine();

            var resultType = GeneratorHelper.MakeGlobalName(TypeHelper.GetEnumerableElementType(mm.EngineResultType));
            AppendLine($"return {EngineFieldRef}.ReaderToDefer<{resultType}>({CommandVar}, {ReaderVar});");
            EndConnectionForReader(mm);

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

            DefineSql(mm);

            DefineConnectionOpen(mm);

            // Execute
            Indent();
            Append($"var {ResultVar} = ");

            var resultType = GeneratorHelper.MakeGlobalName(TypeHelper.GetListElementType(mm.EngineResultType));
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

            DefineSql(mm);

            DefineConnectionOpen(mm);

            // Execute
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
            foreach (var pmi in mm.MethodInfo.GetParameters())
            {
                if (!first)
                {
                    Append(", ");
                }
                else
                {
                    first = false;
                }

                if (pmi.IsOut)
                {
                    Append("out ");
                }
                else if (pmi.ParameterType.IsByRef)
                {
                    Append("ref ");
                }

                var parameterType = pmi.ParameterType.IsByRef ? pmi.ParameterType.GetElementType() : pmi.ParameterType;
                Append($"{GeneratorHelper.MakeGlobalName(parameterType)} {pmi.Name}");
            }

            AppendLine(")");

            AppendLine("{");
            indent++;
        }

        private void BeginConnectionSimple(MethodMetadata mm)
        {
            if (!mm.HasConnectionParameter)
            {
                var providerName = mm.Provider != null ? GetProviderFieldNameRef(mm.No) : ProviderFieldRef;
                AppendLine($"using (var {ConnectionVar} = {providerName}.CreateConnection())");
            }

            AppendLine($"using (var {CommandVar} = {GetConnectionName(mm)}.CreateCommand())");
            AppendLine("{");
            indent++;

            var current = source.Length;

            DefineCommandOption(mm);

            if (source.Length > current)
            {
                NewLine();
            }
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

        private void BeginConnectionForReader(MethodMetadata mm)
        {
            AppendLine($"var {CommandVar} = default({DbCommandType});");
            AppendLine($"var {ReaderVar} = default({DataReaderType});");
            if (mm.HasConnectionParameter)
            {
                AppendLine($"var {WasClosedVar} = {GetConnectionName(mm)}.State == {ConnectionStateType}.Closed;");
            }
            else
            {
                var providerName = mm.Provider != null ? GetProviderFieldNameRef(mm.No) : ProviderFieldRef;
                AppendLine($"var {ConnectionVar} = {providerName}.CreateConnection();");
            }

            AppendLine("try");
            AppendLine("{");
            indent++;
            AppendLine($"{CommandVar} = {GetConnectionName(mm)}.CreateCommand();");

            DefineCommandOption(mm);

            NewLine();
        }

        private void EndConnectionForReader(MethodMetadata mm)
        {
            indent--;
            AppendLine("}");

            AppendLine($"catch ({ExceptionType})");
            AppendLine("{");
            indent++;

            AppendLine($"{ReaderVar}?.Dispose();");
            AppendLine($"{CommandVar}?.Dispose();");
            if (!mm.HasConnectionParameter)
            {
                AppendLine($"{ConnectionVar}.Close();");
            }
            AppendLine("throw;");

            indent--;
            AppendLine("}");

            if (mm.HasConnectionParameter)
            {
                AppendLine("finally");
                AppendLine("{");
                indent++;

                AppendLine($"if ({WasClosedVar})");
                AppendLine("{");
                indent++;
                AppendLine($"{GetConnectionName(mm)}.Close();");
                indent--;
                AppendLine("}");

                indent--;
                AppendLine("}");
            }
        }

        private void DefineCommandOption(MethodMetadata mm)
        {
            if (mm.CommandType != CommandType.Text)
            {
                AppendLine($"{CommandVar}.CommandType = {CommandTypeType}.{mm.CommandType};");
            }

            if (mm.Timeout != null)
            {
                AppendLine($"{CommandVar}.CommandTimeout = {mm.Timeout.Timeout};");
            }
            else if (mm.TimeoutParameter != null)
            {
                AppendLine($"{CommandVar}.CommandTimeout = {mm.TimeoutParameter.Name};");
            }

            if (mm.TransactionParameter != null)
            {
                AppendLine($"{CommandVar}.Transaction = {mm.TransactionParameter.Name};");
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
            var current = source.Length;

            foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input && x.Type != typeof(object)))
            {
                AppendLine($"var {GetOutParamName(parameter.Index)} = default({DbParameterType});");
            }

            if (source.Length > current)
            {
                NewLine();
            }
        }

        private void DefinePostProcess(MethodMetadata mm)
        {
            var first = true;
            foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input))
            {
                if (first)
                {
                    NewLine();
                    first = false;
                }

                Indent();
                Append($"{parameter.Source} = ");

                if (parameter.Type != typeof(object))
                {
                    Append($"{RuntimeHelperType}.Convert<{GeneratorHelper.MakeGlobalName(parameter.Type)}>(");
                    NewLine();
                    indent++;
                    Indent();
                    Append($"{GetOutParamName(parameter.Index)}.Value,");
                    NewLine();
                    Indent();
                    Append($"{GetConvertFieldNameRef(mm.No, parameter.Index)});");
                    indent--;
                }
                else
                {
                    Append($"{GetOutParamName(parameter.Index)}.Value;");
                }

                NewLine();
            }
        }

        private void DefineSql(MethodMetadata mm)
        {
            // TODO
            AppendLine($"{CommandVar}.CommandText = \"***\";");
            NewLine();
        }
    }
}
