using System.Collections.Generic;
using System.Text;

namespace WorkParser
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TestIf();
        }

        // TODO if, if else, for

        // ! 拡張(using、IF(cond)、END、ELSE、ELSE_IF?(cond)、FOR(col in var)
        // @ パラメータ
        // # 置換
        // % コード

        // SQLとパラメータだけなら最適化が可能MaybeDynamic

        private static void TestIf()
        {
            var list = new List<Token>
            {
                new Token("a"),
                new Token("%IF b"),
                new Token("@b"),
                new Token("%END"),
                new Token("c"),
            };


        }
    }

    //--------------------------------------------------------------------------------
    // Token
    //--------------------------------------------------------------------------------

    public class Token
    {
        public string Value { get; }

        public Token(string value)
        {
            Value = value;
        }
    }

    //--------------------------------------------------------------------------------
    // Parser
    //--------------------------------------------------------------------------------

    public interface IParserContext
    {
        void AddNode(INode node);
    }

    public interface IParserHandler
    {
        bool Handle(Token token, IParserContext context);
    }

    public class ParserContext : IParserContext
    {
        private readonly Queue<Token> tokens;

        public List<INode> Nodes { get; } = new List<INode>();

        public bool HasNext => tokens.Count > 0;

        public ParserContext(IEnumerable<Token> tokens)
        {
            this.tokens = new Queue<Token>(tokens);
        }

        public void AddNode(INode node)
        {
            Nodes.Add(node);
        }

        public Token DequeueToken() => tokens.Dequeue();
    }

    public class Parser
    {
        private static readonly List<IParserHandler> handlers = new List<IParserHandler>
        {
            new ImportParserHandler(),
            new CodeParserHandler()
        };

        private readonly List<Token> tokens;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IList<INode> Parse()
        {
            var context = new ParserContext(tokens);

            while (context.HasNext)
            {
                var token = context.DequeueToken();
                foreach (var handler in handlers)
                {
                    if (handler.Handle(token, context))
                    {
                        break;
                    }
                }
            }

            return context.Nodes;
        }
    }

    public class ImportParserHandler : IParserHandler
    {
        public bool Handle(Token token, IParserContext context)
        {
            if (!token.Value.StartsWith("!using "))
            {
                return false;
            }

            context.AddNode(new ImportNode(token.Value.Substring(7).Trim()));
            return true;
        }
    }

    public class CodeParserHandler : IParserHandler
    {
        public bool Handle(Token token, IParserContext context)
        {
            if (!token.Value.StartsWith("%"))
            {
                return false;
            }

            context.AddNode(new CodeNode(token.Value.Substring(1).Trim()));
            return true;
        }
    }

    public class ReplaceParserHandler : IParserHandler
    {
        public bool Handle(Token token, IParserContext context)
        {
            if (!token.Value.Trim().StartsWith("#"))
            {
                return false;
            }

            context.AddNode(new ReplaceNode(token.Value.Substring(1).Trim()));
            return true;
        }
    }

    // TODO パラメータ Peek, ()数が一致するまで, 終了するまでになくなれば例外！

    // ! 拡張 IF(cond)、END、ELSE、ELSE_IF?(cond)、FOR(col in var) コードで書けば同じなので低優先

    // TODO
    // 動的パラメータへの対応、これは引数パラメータとは別枠？、入れ子などの処理と一緒か？
    // for (var item : items) {
    // @item
    // }
    // .で区切られている場合はそこを追っていく、その属性を見る

    //--------------------------------------------------------------------------------
    // Node
    //--------------------------------------------------------------------------------

    public class Parameter
    {
        public int No { get; }

        public string Source { get; }

        public Parameter(int no, string source)
        {
            No = no;
            Source = source;
        }
    }

    public interface IBuildContext
    {
        void AddImport(string value);

        void AddParameter(string value);

        void AddSource(string str);
    }

    public class BuildContext : IBuildContext
    {
        private readonly StringBuilder source = new StringBuilder();

        public IList<string> Imports { get; } = new List<string>();

        public IList<Parameter> Parameters { get; } = new List<Parameter>();

        public void AddImport(string value)
        {
            Imports.Add(value);
        }

        public void AddParameter(string value)
        {
            Parameters.Add(new Parameter(Parameters.Count, value));
        }

        public void AddSource(string str)
        {
            source.Append(str);
        }

        public string GetSource() => source.ToString();
    }

    public interface INode
    {
        bool MaybeDynamic { get; }

        void Build(IBuildContext context);
    }

    public class SqlNode : INode
    {
        private readonly string value;

        public bool MaybeDynamic => false;

        public SqlNode(string value)
        {
            this.value = value;
        }

        public void Build(IBuildContext context)
        {
            context.AddSource($"text(\"{value}\");");
            context.AddSource("\r\n");
        }
    }

    public class CodeNode : INode
    {
        private readonly string value;

        public bool MaybeDynamic => true;

        public CodeNode(string value)
        {
            this.value = value;
        }

        public void Build(IBuildContext context)
        {
            context.AddSource(value);
            context.AddSource("\r\n");
        }
    }

    public class ImportNode : INode
    {
        private readonly string value;

        public bool MaybeDynamic => false;

        public ImportNode(string value)
        {
            this.value = value;
        }

        public void Build(IBuildContext context)
        {
            context.AddImport(value);
        }
    }

    public class ReplaceNode : INode
    {
        private readonly string value;

        public bool MaybeDynamic => true;

        public ReplaceNode(string value)
        {
            this.value = value;
        }

        public void Build(IBuildContext context)
        {
            context.AddSource($"text({value});");
            context.AddSource("\r\n");
        }
    }

    public class ParameterNode : INode
    {
        private readonly string value;

        public bool MaybeDynamic => false;

        public ParameterNode(string value)
        {
            this.value = value;
        }

        public void Build(IBuildContext context)
        {
            context.AddParameter(value);
            context.AddSource($"param({value});");
            context.AddSource("\r\n");

            // TODO IE<>なら複数のコード、この時点で引数飲めたデータにアクセスできている必要あり
            // 動的変数の展開には対応しないよ
        }
    }
}
