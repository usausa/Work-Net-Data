namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;

    using DataLibrary.Engine;

    public class CodeBuilder
    {
        private static readonly string EngineTypeName = $"global::{typeof(ExecuteEngine).Namespace}.{typeof(ExecuteEngine).Name}";

        private readonly StringBuilder source = new StringBuilder();

        private readonly HashSet<Assembly> references;

        private bool newLine = true;

        public int Indent { get; set; }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public CodeBuilder()
        {
            references = new HashSet<Assembly>
            {
                Assembly.Load("System.Runtime"),
                Assembly.Load("netstandard"),
                typeof(object).Assembly,
                typeof(ExecuteEngine).Assembly,
            };
        }

        //--------------------------------------------------------------------------------
        // Builder
        //--------------------------------------------------------------------------------

        public string GetSource() => source.ToString();

        public IEnumerable<Assembly> GetReferences() => references;

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private void AppendIndent()
        {
            if (newLine)
            {
                source.Append(new String(' ', Indent * 4));
                newLine = false;
            }
        }

        private CodeBuilder Append(string code)
        {
            AppendIndent();
            source.Append(code);
            return this;
        }

        // TODO private
        public CodeBuilder AppendLine(string code)
        {
            AppendIndent();
            source.AppendLine(code);
            newLine = true;
            return this;
        }

        private CodeBuilder AppendType(Type type)
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
        // Operation
        //--------------------------------------------------------------------------------

        public CodeBuilder NewLine()
        {
            source.AppendLine();
            newLine = true;
            return this;
        }

        public void End()
        {
            Indent--;
            AppendLine("}");
        }

        public void BeginNamespace(string ns)
        {
            Append("namespace ").AppendLine(ns);
            AppendLine("{");
            Indent++;
        }

        //public CodeBuilder AppendUsing(string ns)
        //{
        //    Append("using ").Append(ns).AppendLine(";");
        //}

        //// TODO static

        public void BeginClass(string className, Type baseType)
        {
            Append("internal sealed class ").Append(className).Append(" : ").AppendType(baseType).NewLine();
            AppendLine("{");
            Indent++;
        }

        public void DefineEngineField()
        {
            Append("private readonly ").Append(EngineTypeName).AppendLine(" _engine;");
        }

        public void AppendField(Type type, string memberName)
        {
            Append("private readonly ").AppendType(type).Append(" ").Append(memberName).AppendLine(";");
        }

        public void BeginCtor(string className)
        {
            Append("public ").Append(className).Append("(").Append(EngineTypeName).AppendLine(" engine)");
            AppendLine("{");
            Indent++;

            AppendLine("this._engine = engine;");
        }

        public void BeginMethod(MethodMetadata mm)
        {
            Append("public ");
            if (mm.Result.IsAsync)
            {
                Append("async ");
            }

            AppendType(mm.Result.Type).Append(" ").Append(mm.Name).Append("(");

            var first = true;
            foreach (var pm in mm.Parameters)
            {
                if (!first)
                {
                    Append(", ");
                }
                else
                {
                    first = false;
                }

                if (pm.IsOut)
                {
                    Append("out ");
                }
                else if (pm.IsRef)
                {
                    Append("ref ");
                }

                AppendType(pm.IsRef ? pm.Type.GetElementType() : pm.Type).Append(" ").Append(pm.Name);
            }

            AppendLine(")");

            AppendLine("{");
            Indent++;
        }
    }
}
