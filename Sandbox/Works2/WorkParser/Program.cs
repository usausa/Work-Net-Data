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
                new Token(TokenType.Comment, "%using System"),
                new Token(TokenType.Comment, "%helper Library.Helper"),
                new Token(TokenType.Block  , "SELECT * FROM Test WHERE 1 = 1"),
                new Token(TokenType.Comment, "% if (name != null) {"),
                new Token(TokenType.Block  , "AND Name ="),
                new Token(TokenType.Comment, "@name"),
                new Token(TokenType.Block  , "'test'"),
                new Token(TokenType.Comment, "% }"),
            };

            var parser = new Parser();
            // TODO コンテキスト(パラメータ)
            var methodMetadata = new MethodMetadata();
            var result = parser.Parse(methodMetadata, list);
            //parser.
        }
    }

    //--------------------------------------------------------------------------------
    // Token
    //--------------------------------------------------------------------------------

    public enum TokenType
    {
        Comment,
        Block,
        OpenParenthesis,
        CloseParenthesis
    }

    public sealed class Token
    {
        public TokenType TokenType { get; }

        public string Value { get; }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
    }

    //--------------------------------------------------------------------------------
    // Parser
    //--------------------------------------------------------------------------------

    // TODO Parserが直接Buildする方式はだめ、最適化ができないから

    public class MethodMetadata
    {
        // TODO parameter型、引数パラメータの最適化用に必要？、いや決まっているはずだからContextから取得か？
    }

    public class ImportEntry
    {
        public bool Helper { get; }

        public string Value { get; }

        public ImportEntry(bool helper, string value)
        {
            Helper = helper;
            Value = value;
        }
    }

    public class ParseResult
    {
        private readonly StringBuilder source = new StringBuilder();

        public bool MaybeDynamic { get; set; }

        public IList<ImportEntry> Imports { get; } = new List<ImportEntry>();

        public void AddImport(bool helper, string value)
        {
            Imports.Add(new ImportEntry(helper, value));
        }

        public void AddSource(string str)
        {
            source.AppendLine(str);
        }

        public string GetSource() => source.ToString();
    }

    public interface IParserHandler
    {
        bool Handle(Token token, ParserContext context);
    }

    public class ParserContext
    {
        private readonly Queue<Token> tokens;

        public MethodMetadata MethodMetadata { get; }

        public ParseResult Result { get; } = new ParseResult();

        public bool HasNext => tokens.Count > 0;

        public ParserContext(MethodMetadata methodMetadata, IEnumerable<Token> tokens)
        {
            MethodMetadata = methodMetadata;
            this.tokens = new Queue<Token>(tokens);
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

        public ParseResult Parse(MethodMetadata methodMetadata, IEnumerable<Token> tokens)
        {
            var context = new ParserContext(methodMetadata, tokens);

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

            return context.Result;
        }
    }

    public class ImportParserHandler : IParserHandler
    {
        public bool Handle(Token token, ParserContext context)
        {
            if ((token.TokenType != TokenType.Comment) || !token.Value.StartsWith("%using "))
            {
                return false;
            }

            context.Result.AddImport(false, token.Value.Substring(7).Trim());
            return true;
        }
    }

    public class HelperParserHandler : IParserHandler
    {
        public bool Handle(Token token, ParserContext context)
        {
            if ((token.TokenType != TokenType.Comment) || !token.Value.StartsWith("%using "))
            {
                return false;
            }

            context.Result.AddImport(true, token.Value.Substring(7).Trim());
            return true;
        }
    }

    public class CodeParserHandler : IParserHandler
    {
        public bool Handle(Token token, ParserContext context)
        {
            if ((token.TokenType != TokenType.Comment) || !token.Value.StartsWith("%"))
            {
                return false;
            }

            context.Result.AddSource(token.Value.Substring(1).Trim());
            context.Result.MaybeDynamic = true;
            return true;
        }
    }

    //public class ReplaceParserHandler : IParserHandler
    //{
    //    public bool Handle(Token token, IParserContext context)
    //    {
    //        if (!token.Value.Trim().StartsWith("#"))
    //        {
    //            return false;
    //        }

    //        context.AddNode(new ReplaceNode(token.Value.Substring(1).Trim()));
    //        return true;
    //    }
    //}

    // TODO パラメータ Peek, ()数が一致するまで, 終了するまでになくなれば例外！

    // ! 拡張 IF(cond)、END、ELSE、ELSE_IF?(cond)、FOR(col in var) コードで書けば同じなので低優先

    // TODO
    // 動的パラメータへの対応、これは引数パラメータとは別枠？、入れ子などの処理と一緒か？
    // for (var item : items) {
    // @item
    // }
    // .で区切られている場合はそこを追っていく、その属性を見る

    //public interface INode
    //{
    //    bool MaybeDynamic { get; }

    //    void Build(IBuildContext context);
    //}

    //public class SqlNode : INode
    //{
    //    private readonly string value;

    //    public bool MaybeDynamic => false;

    //    public SqlNode(string value)
    //    {
    //        this.value = value;
    //    }

    //    public void Build(IBuildContext context)
    //    {
    //        context.AddSource($"text(\"{value}\");");
    //    }
    //}

    //public class ReplaceNode : INode
    //{
    //    private readonly string value;

    //    public bool MaybeDynamic => true;

    //    public ReplaceNode(string value)
    //    {
    //        this.value = value;
    //    }

    //    public void Build(IBuildContext context)
    //    {
    //        context.AddSource($"text({value});");
    //    }
    //}

    //public class ParameterNode : INode
    //{
    //    private readonly string value;

    //    public bool MaybeDynamic => false;

    //    public ParameterNode(string value)
    //    {
    //        this.value = value;
    //    }

    //    public void Build(IBuildContext context)
    //    {
    //        context.AddParameter(value);
    //        context.AddSource($"param({value});");
    //        context.AddSource("\r\n");

    //        // TODO IE<>なら複数のコード、この時点で引数飲めたデータにアクセスできている必要あり
    //        // 動的変数の展開には対応しないよ
    //    }
    //}
}
