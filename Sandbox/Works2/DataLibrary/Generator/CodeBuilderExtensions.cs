namespace DataLibrary.Generator
{
    public static class CodeBuilderExtensions
    {
        public static CodeBuilder End(this CodeBuilder builder)
        {
            builder.Indent--;
            builder.WriteLine("}");
            return builder;
        }

        public static CodeBuilder BeginNamespace(this CodeBuilder builder, string ns)
        {
            builder.Write("namespace ").WriteLine(ns);
            builder.WriteLine("{");
            builder.Indent++;
            return builder;
        }

        //public static CodeBuilder WriteUsing(this CodeBuilder builder, string ns)
        //{
        //    builder.Write("using ").Write(ns).WriteLine(";");
        //    return builder;
        //}

        //// TODO static

        public static CodeBuilder BeginClass(this CodeBuilder builder, string className, string baseClassName)
        {
            builder.Write("internal sealed class ").Write(className).Write(" : global::").WriteLine(baseClassName);
            builder.WriteLine("{");
            builder.Indent++;
            return builder;
        }
    }
}
