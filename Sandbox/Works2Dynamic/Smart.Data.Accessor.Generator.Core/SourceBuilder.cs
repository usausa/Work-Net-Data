namespace Smart.Data.Accessor.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Data.Accessor.Generator.Helpers;
    using Smart.Data.Accessor.Generator.Metadata;
    using Smart.Data.Accessor.Generator.Visitors;
    using Smart.Data.Accessor.Nodes;
    using Smart.Data.Accessor.Runtime;
    using Smart.Data.Accessor.Scripts;

    internal sealed class SourceBuilder
    {
        private const string CtorArg = "engine";
        private const string EngineField = "_engine";
        private const string EngineFieldRef = "this." + EngineField;
        private const string ProviderField = "_provider";
        private const string ProviderFieldRef = "this." + ProviderField;
        private const string HandlerField = "_handler";
        private const string SetupReturnField = "_setupReturn";
        private const string SetupParameterField = "_setupParameter";
        private const string SetupSqlField = "_setupSql";

        private const string ConnectionVar = "_con";
        private const string CommandVar = "_cmd";
        private const string ReaderVar = "_reader";
        private const string ResultVar = "_result";
        private const string BuilderVar = "_sql";
        private const string OutParamVar = "_outParam";
        private const string ReturnOutParamVar = "_outParamReturn";
        private const string FlagVar = "_flag";
        private const string SqlVar = "_sql";

        private static readonly string EngineType = GeneratorHelper.MakeGlobalName(typeof(ExecuteEngine));
        private static readonly string RuntimeHelperType = GeneratorHelper.MakeGlobalName(typeof(RuntimeHelper));
        private static readonly string MethodNoAttributeType = GeneratorHelper.MakeGlobalName(typeof(MethodNoAttribute));
        private static readonly string ProviderType = GeneratorHelper.MakeGlobalName(typeof(IDbProvider));
        private static readonly string DbDataReaderType = GeneratorHelper.MakeGlobalName(typeof(DbDataReader));
        private static readonly string DbCommandType = GeneratorHelper.MakeGlobalName(typeof(DbCommand));
        private static readonly string DbParameterType = GeneratorHelper.MakeGlobalName(typeof(DbParameter));
        private static readonly string CommandTypeType = GeneratorHelper.MakeGlobalName(typeof(CommandType));
        private static readonly string WrappedReaderType = GeneratorHelper.MakeGlobalName(typeof(WrappedReader));
        private static readonly string StringBuilderType = GeneratorHelper.MakeGlobalName(typeof(StringBuilder));
        private static readonly string ExceptionType = GeneratorHelper.MakeGlobalName(typeof(Exception));
        private static readonly string HandlerType = GeneratorHelper.MakeGlobalName(typeof(Func<object, object>));
        private static readonly string OutSetupType = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, string, DbParameter>));
        private static readonly string ReturnSetupType = GeneratorHelper.MakeGlobalName(typeof(Func<DbCommand, DbParameter>));

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

        internal SourceBuilder(Type targetType, string implementName)
        {
            this.targetType = targetType;
            this.implementName = implementName;

            interfaceFullName = GeneratorHelper.MakeGlobalName(targetType);
            provider = targetType.GetCustomAttribute<ProviderAttribute>();
        }

        public void AddMethod(MethodMetadata mm)
        {
            methods.Add(mm);
        }

        //--------------------------------------------------------------------------------
        // Build
        //--------------------------------------------------------------------------------

        public string Build()
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

            return source.ToString();
        }

        //--------------------------------------------------------------------------------
        // Naming
        //--------------------------------------------------------------------------------

        private static string GetProviderFieldName(int no) => ProviderField + no;

        private static string GetProviderFieldRef(int no) => "this." + GetProviderFieldName(no);

        private static string GetHandlerFieldName(int no) => HandlerField + no;

        private static string GetHandlerFieldRef(int no) => "this." + GetHandlerFieldName(no);

        private static string GetHandlerFieldName(int no, int index) => HandlerField + no + "_" + index;

        private static string GetHandlerFieldRef(int no, int index) => "this." + GetHandlerFieldName(no, index);

        private static string GetSetupReturnFieldName(int no) => SetupReturnField + no;

        private static string GetSetupReturnFieldRef(int no) => "this." + GetSetupReturnFieldName(no);

        private static string GetSetupParameterFieldName(int no, int index) => SetupParameterField + no + "_" + index;

        private static string GetSetupParameterFieldRef(int no, int index) => "this." + GetSetupParameterFieldName(no, index);

        private static string GetSetupSqlFieldName(int no, int index) => SetupSqlField + no + "_" + index;

        private static string GetSetupSqlFieldRef(int no, int index) => "this." + GetSetupSqlFieldName(no, index);

        private static string GetOutParamName(int index) => OutParamVar + index;

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private static bool IsResultConverterRequired(MethodMetadata mm)
        {
            return (((mm.MethodType == MethodType.Execute) && mm.ReturnValueAsResult) ||
                    (mm.MethodType == MethodType.ExecuteScalar)) &&
                   (mm.EngineResultType != typeof(object) &&
                    (mm.EngineResultType != typeof(void)));
        }

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
        // Source
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
                    if (!IsValidExecuteResultType(mm.EngineResultType, mm.ReturnValueAsResult))
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

        private static bool IsValidExecuteResultType(Type type, bool returnValueAsResult)
        {
            return returnValueAsResult || type == typeof(int) || type == typeof(void);
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
            AppendLine("using System;");
            AppendLine("using System.Linq;");

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

            AppendLine($"using static {typeof(ScriptHelper).FullName.Replace('+', '.')};");

            NewLine();
        }

        private void BeginClass(string className)
        {
            AppendLine($"public sealed class {className} : {interfaceFullName}");
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

                if (IsResultConverterRequired(mm))
                {
                    AppendLine($"private readonly {HandlerType} {GetHandlerFieldName(mm.No)};");
                }

                if (mm.ReturnValueAsResult)
                {
                    AppendLine($"private readonly {ReturnSetupType} {GetSetupReturnFieldName(mm.No)};");
                }

                foreach (var parameter in mm.Parameters)
                {
                    switch (parameter.Direction)
                    {
                        case ParameterDirection.ReturnValue:
                            AppendLine($"private readonly {ReturnSetupType} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterDirection.Output:
                            AppendLine($"private readonly {OutSetupType} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterDirection.InputOutput:
                            AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeInOutParameterType(parameter.Type))} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterDirection.Input:
                            switch (parameter.ParameterType)
                            {
                                case ParameterType.Array:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeArrayParameterType(parameter.Type))} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                                    break;
                                case ParameterType.List:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeListParameterType(parameter.Type))} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                                    break;
                                default:
                                    AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeInParameterType(parameter.Type))} {GetSetupParameterFieldName(mm.No, parameter.Index)};");
                                    break;
                            }
                            break;
                    }
                }

                foreach (var parameter in mm.Parameters)
                {
                    switch (parameter.ParameterType)
                    {
                        case ParameterType.Array:
                            AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeArraySqlType(parameter.Type))} {GetSetupSqlFieldName(mm.No, parameter.Index)};");
                            break;
                        case ParameterType.List:
                            AppendLine($"private readonly {GeneratorHelper.MakeGlobalName(GeneratorHelper.MakeListSqlType(parameter.Type))} {GetSetupSqlFieldName(mm.No, parameter.Index)};");
                            break;
                    }
                }

                foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input && x.Type != typeof(object)))
                {
                    AppendLine($"private readonly {HandlerType} {GetHandlerFieldName(mm.No, parameter.Index)};");
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

            var useDefaultProvider = methods.Any(x => (x.ConnectionParameter == null) && (x.TransactionParameter == null));
            if (useDefaultProvider)
            {
                NewLine();
                if (provider == null)
                {
                    AppendLine($"{ProviderFieldRef} = ({ProviderType}){CtorArg}.ServiceProvider.GetService(typeof({ProviderType}));");
                }
                else
                {
                    AppendLine($"{ProviderFieldRef} = {RuntimeHelperType}.GetDbProvider({CtorArg}, typeof({interfaceFullName}));");
                }
            }

            // Per method
            foreach (var mm in methods)
            {
                var hasProvider = mm.Provider != null;
                var hasConverter = IsResultConverterRequired(mm);
                if (hasProvider || hasConverter || mm.ReturnValueAsResult || mm.Parameters.Count > 0)
                {
                    NewLine();
                    AppendLine($"var method{mm.No} = {RuntimeHelperType}.GetInterfaceMethodByNo(GetType(), typeof({interfaceFullName}), {mm.No});");

                    if (hasProvider)
                    {
                        AppendLine($"{GetProviderFieldRef(mm.No)} = {RuntimeHelperType}.GetDbProvider({CtorArg}, method{mm.No});");
                    }

                    if (hasConverter)
                    {
                        AppendLine($"{GetHandlerFieldRef(mm.No)} = {CtorArg}.CreateHandler<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(method{mm.No});");
                    }

                    if (mm.ReturnValueAsResult)
                    {
                        AppendLine($"{GetSetupReturnFieldRef(mm.No)} = {CtorArg}.CreateReturnParameterSetup();");
                    }

                    foreach (var parameter in mm.Parameters)
                    {
                        Indent();
                        Append($"{GetSetupParameterFieldRef(mm.No, parameter.Index)} = ");

                        switch (parameter.Direction)
                        {
                            case ParameterDirection.ReturnValue:
                                Append($"{CtorArg}.CreateReturnParameterSetup();");
                                break;
                            case ParameterDirection.Output:
                                Append($"{RuntimeHelperType}.CreateOutParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                break;
                            case ParameterDirection.InputOutput:
                                Append($"{RuntimeHelperType}.CreateInOutParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                break;
                            case ParameterDirection.Input:
                                switch (parameter.ParameterType)
                                {
                                    case ParameterType.Array:
                                        Append($"{RuntimeHelperType}.CreateArrayParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type.GetElementType())}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                    case ParameterType.List:
                                        Append($"{RuntimeHelperType}.CreateListParameterSetup<{GeneratorHelper.MakeGlobalName(TypeHelper.GetListElementType(parameter.Type))}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                    default:
                                        Append($"{RuntimeHelperType}.CreateInParameterSetup<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
                                        break;
                                }
                                break;
                        }

                        NewLine();
                    }

                    foreach (var parameter in mm.Parameters)
                    {
                        if (parameter.ParameterType != ParameterType.Simple)
                        {
                            Indent();
                            Append($"{GetSetupSqlFieldRef(mm.No, parameter.Index)} = ");

                            switch (parameter.ParameterType)
                            {
                                case ParameterType.Array:
                                    Append($"{CtorArg}.CreateArraySqlSetup<{GeneratorHelper.MakeGlobalName(parameter.Type.GetElementType())}>();");
                                    break;
                                case ParameterType.List:
                                    Append($"{CtorArg}.CreateListSqlSetup<{GeneratorHelper.MakeGlobalName(TypeHelper.GetListElementType(parameter.Type))}>();");
                                    break;
                            }

                            NewLine();
                        }
                    }

                    foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input && x.Type != typeof(object)))
                    {
                        AppendLine($"{GetHandlerFieldRef(mm.No, parameter.Index)} = {RuntimeHelperType}.CreateHandler<{GeneratorHelper.MakeGlobalName(parameter.Type)}>({CtorArg}, method{mm.No}, \"{parameter.Source}\");");
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

            if (mm.ReturnValueAsResult && (mm.EngineResultType != typeof(void)))
            {
                AppendLine($"var {ReturnOutParamVar} = {GetSetupReturnFieldRef(mm.No)}({CommandVar});");
                NewLine();
            }

            DefineConnectionOpen(mm);

            // Body
            Indent();

            if (!mm.ReturnValueAsResult && (mm.EngineResultType != typeof(void)))
            {
                Append($"var {ResultVar} = ");
            }

            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteAsync({CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.Execute({CommandVar});");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            if (mm.ReturnValueAsResult && (mm.EngineResultType != typeof(void)))
            {
                if (mm.EngineResultType != typeof(object))
                {
                    NewLine();

                    Indent();
                    Append($"var {ResultVar} = {EngineFieldRef}.Convert<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(");
                    NewLine();
                    indent++;
                    Indent();
                    Append($"{ReturnOutParamVar}.Value,");
                    NewLine();
                    Indent();
                    Append($"{GetHandlerFieldRef(mm.No)});");
                    indent--;
                    NewLine();
                }
                else
                {
                    NewLine();
                    AppendLine($"var {ResultVar} = {ReturnOutParamVar}.Value;");
                }
            }

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
                Append($"{EngineFieldRef}.Convert<{GeneratorHelper.MakeGlobalName(mm.EngineResultType)}>(");
                NewLine();
                indent++;
                Indent();
            }

            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteScalarAsync({CommandVar}{cancelOption}).ConfigureAwait(false)");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteScalar({CommandVar})");
            }

            if (mm.EngineResultType != typeof(object))
            {
                Append(",");
                NewLine();
                Indent();
                Append($"{GetHandlerFieldRef(mm.No)});");
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
            Append($"{ReaderVar} = ");

            var closeOption = mm.HasConnectionParameter ? string.Empty : "WithClose";
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteReader{closeOption}Async({CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteReader{closeOption}({CommandVar});");
            }

            NewLine();

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
            Append($"{ReaderVar} = ");

            var closeOption = mm.HasConnectionParameter ? string.Empty : "WithClose";
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.ExecuteReader{closeOption}Async({CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.ExecuteReader{closeOption}({CommandVar});");
            }

            NewLine();

            // PostProcess
            DefinePostProcess(mm);

            NewLine();

            var resultType = GeneratorHelper.MakeGlobalName(TypeHelper.GetEnumerableElementType(mm.EngineResultType));
            AppendLine($"var {ResultVar} = {EngineFieldRef}.ReaderToDefer<{resultType}>({CommandVar}, {ReaderVar});");
            AppendLine($"{CommandVar} = null;");
            AppendLine($"{ReaderVar} = null;");
            AppendLine($"return {ResultVar};");
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
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.QueryBufferAsync<{resultType}>({CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.QueryBuffer<{resultType}>({CommandVar});");
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
            if (mm.IsAsync)
            {
                var cancelOption = mm.CancelParameter != null ? $", {mm.CancelParameter.Name}" : string.Empty;
                Append($"await {EngineFieldRef}.QueryFirstOrDefaultAsync<{resultType}>({CommandVar}{cancelOption}).ConfigureAwait(false);");
            }
            else
            {
                Append($"{EngineFieldRef}.QueryFirstOrDefault<{resultType}>({CommandVar});");
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
                var providerName = mm.Provider != null ? GetProviderFieldRef(mm.No) : ProviderFieldRef;
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
            AppendLine($"var {ReaderVar} = default({DbDataReaderType});");
            if (!mm.HasConnectionParameter)
            {
                var providerName = mm.Provider != null ? GetProviderFieldRef(mm.No) : ProviderFieldRef;
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
                AppendLine($"{ConnectionVar}.Dispose();");
            }
            AppendLine("throw;");

            indent--;
            AppendLine("}");
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

            foreach (var parameter in mm.Parameters.Where(x => x.Direction != ParameterDirection.Input))
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
                    Append($"{EngineFieldRef}.Convert<{GeneratorHelper.MakeGlobalName(parameter.Type)}>(");
                    NewLine();
                    indent++;
                    Indent();
                    Append($"{GetOutParamName(parameter.Index)}.Value,");
                    NewLine();
                    Indent();
                    Append($"{GetHandlerFieldRef(mm.No, parameter.Index)});");
                    indent--;
                }
                else
                {
                    Append($"{GetOutParamName(parameter.Index)}.Value;");
                }

                NewLine();
            }
        }

        //--------------------------------------------------------------------------------
        // SQL
        //--------------------------------------------------------------------------------

        private void DefineSql(MethodMetadata mm)
        {
            if (mm.CommandType == CommandType.StoredProcedure)
            {
                var visitor = new ProcedureBuildVisitor(this, mm);
                visitor.Visit(mm.Nodes);
            }
            else
            {
                var checkVisitor = new DynamicCheckVisitor();
                checkVisitor.Visit(mm.Nodes);

                if (checkVisitor.IsDynamic)
                {
                    var calc = new CalcSizeVisitor(mm);
                    calc.Visit(mm.Nodes);

                    var visitor = new DynamicBuildVisitor(this, mm, calc.InitialSize);
                    visitor.Visit(mm.Nodes);
                    visitor.Flush();
                }
                else if (mm.Parameters.Any(x => x.ParameterType != ParameterType.Simple))
                {
                    var calc = new CalcSizeVisitor(mm);
                    calc.Visit(mm.Nodes);

                    var visitor = new HasArrayBuildVisitor(this, mm, calc.InitialSize);
                    visitor.Visit(mm.Nodes);
                    visitor.Flush();
                }
                else
                {
                    var visitor = new SimpleBuildVisitor(this, mm);
                    visitor.Visit(mm.Nodes);
                    visitor.Flush();
                }
            }

            NewLine();
        }

        //--------------------------------------------------------------------------------
        // Procedure
        //--------------------------------------------------------------------------------

        private sealed class ProcedureBuildVisitor : NodeVisitorBase
        {
            private readonly SourceBuilder builder;

            private readonly MethodMetadata mm;

            public ProcedureBuildVisitor(SourceBuilder builder, MethodMetadata mm)
            {
                this.builder = builder;
                this.mm = mm;
            }

            public override void Visit(SqlNode node)
            {
                builder.AppendLine($"{CommandVar}.CommandText = \"{node.Sql}\";");
                builder.NewLine();
            }

            public override void Visit(ParameterNode node)
            {
                var parameter = mm.Parameters.First(x => x.Source == node.Source);
                builder.AppendLine(MakeParameterSetup(mm, parameter, node.ParameterName));
            }
        }

        //--------------------------------------------------------------------------------
        // Simple
        //--------------------------------------------------------------------------------

        private sealed class SimpleBuildVisitor : NodeVisitorBase
        {
            private readonly SourceBuilder builder;

            private readonly MethodMetadata mm;

            private readonly StringBuilder sql = new StringBuilder();

            public SimpleBuildVisitor(SourceBuilder builder, MethodMetadata mm)
            {
                this.builder = builder;
                this.mm = mm;
            }

            public override void Visit(SqlNode node) => sql.Append(node.Sql);

            public override void Visit(ParameterNode node)
            {
                var parameter = mm.Parameters.First(x => x.Source == node.Source);
                var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);
                sql.Append("@");
                sql.Append(parameterName);
            }

            public void Flush()
            {
                var current = builder.source.Length;

                foreach (var parameter in mm.Parameters)
                {
                    var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);
                    builder.AppendLine(MakeParameterSetup(mm, parameter, parameterName));
                }

                if (builder.source.Length > current)
                {
                    builder.NewLine();
                }

                builder.AppendLine($"{CommandVar}.CommandText = \"{sql}\";");
            }
        }

        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        private sealed class HasArrayBuildVisitor : NodeVisitorBase
        {
            private readonly SourceBuilder builder;

            private readonly MethodMetadata mm;

            private readonly StringBuilder sql = new StringBuilder();

            public HasArrayBuildVisitor(SourceBuilder builder, MethodMetadata mm, int size)
            {
                this.builder = builder;
                this.mm = mm;

                builder.AppendLine($"var {SqlVar} = new {StringBuilderType}({size});");
                builder.NewLine();
            }

            public override void Visit(SqlNode node)
            {
                sql.Append(node.Sql);
            }

            public override void Visit(ParameterNode node)
            {
                var parameter = mm.Parameters.First(x => x.Source == node.Source);
                var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);

                if (parameter.ParameterType == ParameterType.Simple)
                {
                    sql.Append($"@{parameterName}");
                }
                else
                {
                    FlushSql();
                    builder.AppendLine(MakeSqlSetup(mm, parameter, $"@{parameterName}"));
                }
            }

            private void FlushSql()
            {
                if (sql.Length > 0)
                {
                    builder.AppendLine($"{SqlVar}.Append(\"{sql}\");");
                }

                sql.Clear();
            }

            public void Flush()
            {
                FlushSql();

                builder.NewLine();

                var current = builder.source.Length;

                foreach (var parameter in mm.Parameters)
                {
                    var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);
                    builder.AppendLine(MakeParameterSetup(mm, parameter, parameterName));
                }

                if (builder.source.Length > current)
                {
                    builder.NewLine();
                }

                builder.AppendLine($"{CommandVar}.CommandText = {SqlVar}.ToString();");
            }
        }

        //--------------------------------------------------------------------------------
        // Dynamic
        //--------------------------------------------------------------------------------

        private sealed class DynamicBuildVisitor : NodeVisitorBase
        {
            private readonly SourceBuilder builder;

            private readonly MethodMetadata mm;

            private readonly StringBuilder sql = new StringBuilder();

            public DynamicBuildVisitor(SourceBuilder builder, MethodMetadata mm, int size)
            {
                this.builder = builder;
                this.mm = mm;

                var current = builder.source.Length;

                foreach (var parameter in mm.Parameters)
                {
                    builder.AppendLine($"var {FlagVar}{parameter.Index} = false;");
                }

                if (builder.source.Length > current)
                {
                    builder.NewLine();
                }

                builder.AppendLine($"var {SqlVar} = new {StringBuilderType}({size});");
                builder.NewLine();
            }

            public override void Visit(SqlNode node)
            {
                sql.Append(node.Sql);
            }

            public override void Visit(RawSqlNode node)
            {
                FlushSql();
                builder.AppendLine($"{SqlVar}.Append({node.Source});");
            }

            public override void Visit(CodeNode node)
            {
                FlushSql();
                builder.AppendLine(node.Code);
            }

            public override void Visit(ParameterNode node)
            {
                var parameter = mm.Parameters.First(x => x.Source == node.Source);
                var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);

                if (parameter.ParameterType == ParameterType.Simple)
                {
                    sql.Append($"@{parameterName}");
                }
                else
                {
                    FlushSql();
                    builder.AppendLine(MakeSqlSetup(mm, parameter, $"@{parameterName}"));
                }
                builder.AppendLine($"{FlagVar}{parameter.Index} = true;");
            }

            private void FlushSql()
            {
                if (sql.Length > 0)
                {
                    builder.AppendLine($"{SqlVar}.Append(\"{sql}\");");
                }

                sql.Clear();
            }

            public void Flush()
            {
                FlushSql();

                builder.NewLine();

                var current = builder.source.Length;

                foreach (var parameter in mm.Parameters)
                {
                    builder.AppendLine($"if ({FlagVar}{parameter.Index})");
                    builder.AppendLine("{");
                    builder.indent++;

                    var parameterName = parameter.ParameterName ?? ParameterNames.GetParameterName(parameter.Index);
                    builder.AppendLine(MakeParameterSetup(mm, parameter, parameterName));

                    builder.indent--;
                    builder.AppendLine("}");
                }

                if (builder.source.Length > current)
                {
                    builder.NewLine();
                }

                builder.AppendLine($"{CommandVar}.CommandText = {SqlVar}.ToString();");
            }
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private static string MakeSqlSetup(MethodMetadata mm, ParameterEntry parameter, string name)
        {
            return $"{GetSetupSqlFieldRef(mm.No, parameter.Index)}(\"{name}\", {BuilderVar}, {parameter.Source});";
        }

        private static string MakeParameterSetup(MethodMetadata mm, ParameterEntry parameter, string name)
        {
            switch (parameter.Direction)
            {
                case ParameterDirection.ReturnValue:
                    return $"{GetOutParamName(parameter.Index)} = {GetSetupParameterFieldRef(mm.No, parameter.Index)}({CommandVar});";
                case ParameterDirection.Output:
                    return $"{GetOutParamName(parameter.Index)} = {GetSetupParameterFieldRef(mm.No, parameter.Index)}({CommandVar}, \"{name}\");";
                case ParameterDirection.InputOutput:
                    return $"{GetOutParamName(parameter.Index)} = {GetSetupParameterFieldRef(mm.No, parameter.Index)}({CommandVar}, \"{name}\", {parameter.Source});";
                default:
                    return $"{GetSetupParameterFieldRef(mm.No, parameter.Index)}({CommandVar}, \"{name}\", {parameter.Source});";
            }
        }
    }
}