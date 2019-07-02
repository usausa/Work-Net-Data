namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Nodes;
    using DataLibrary.Parser;
    using DataLibrary.Tokenizer;

    public abstract class LoaderMethodAttribute : MethodAttribute
    {
        protected LoaderMethodAttribute(MethodType methodType)
            : base(CommandType.Text, methodType)
        {
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            var sql = loader.Load(mi);
            var tokenizer = new SqlTokenizer(sql);
            var tokens = tokenizer.Tokenize();
            var parser = new NodeParser(tokens);
            return parser.Parse();
        }
    }
}
