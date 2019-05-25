using System;
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

        // TODO ローカルバインド変数宣言？

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
            methodMetadata.AddParameter("name", typeof(string));
            var result = parser.Parse(methodMetadata, list);
            //parser.

            // TODO dynamic check
        }
    }

    //--------------------------------------------------------------------------------
    // Builder
    //--------------------------------------------------------------------------------

    public class DefaultMethodBuilder : IMethodBuilder
    {

        public void AddSql(string sql)
        {
            throw new NotImplementedException();
        }

        public void AddSource(string source)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {

        }

        public string GetSource()
        {
            // TODO 通常版 1st
            throw new NotImplementedException();
        }
    }

    public class OptimizedMethodBuilder : IMethodBuilder
    {
        public void AddSql(string sql)
        {
            throw new NotImplementedException();
        }

        public void AddSource(string source)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {

        }

        public string GetSource()
        {
            // TODO 最適化版 1st
            throw new NotImplementedException();
        }
    }

    //--------------------------------------------------------------------------------
    // Parser
    //--------------------------------------------------------------------------------

    // TODO parameter型、引数パラメータの最適化用に必要？、いや決まっているはずだからContextから取得か？
    // TODO Parserが直接Buildする方式はだめ、最適化ができないから?

    public class MethodMetadata
    {
        private readonly Dictionary<string, Type> parameters = new Dictionary<string, Type>();

        public void AddParameter(string parameter, Type type)
        {
            parameters.Add(parameter, type);
        }

        public bool ContainsParameter(string parameter) => parameter.Contains(parameter);
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

    public interface IParserHandler
    {
        bool Handle(Token token, ParserContext context);
    }

    // TODO コンテキスト統合
    public class ParseResult
    {
        // TODO Block(code, sql)
        // Dynamic? sqlとsourceのテンポラリ
        // パラメータはダイナミックと固定、ダイナミックビルダーは単一インスタンス
        // ではなくローカルか？、いや、実際の情報は都度評価しないとダメか？
        // %parameterとかで事前定義を必須にする？

        private readonly StringBuilder sqlTemporary = new StringBuilder();

        private readonly StringBuilder sourceTemporary = new StringBuilder();

        // TODO パラメータもテンポラリ化？

        public IList<ImportEntry> Imports { get; } = new List<ImportEntry>();

        public IList<IBlock> Blocks { get; } = new List<IBlock>();

        public void AddImport(bool helper, string value)
        {
            Imports.Add(new ImportEntry(helper, value));
        }

        public void AddSource(string str)
        {
            // TODO 前処理(未確定SQL、(パラメータを処理))

            //source.AppendLine(str);
        }

        public void AddSql(string str)
        {
            // TODO 前処理(未確定ソース確定)

            //source.AppendLine(str);
        }

        // TODO AddParameter(with sql) パラメータチェック？、遅延させる？

        //public string GetSource() => source.ToString();

        public void Flush()
        {
        }
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

        // TODO 戻りはブロックリスト
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

    // TODO Visitorは拡張可能でない？、その拡張必要？
    // TODO パラメータが配列ならDynamicになる
    // TODO クラスを呼び返すVisitorではなく、メソッドAddSqlとかを呼ぶ形のビルダー？
    // TODO replace ?

    public interface IMethodBuilder
    {
        void AddSql(string sql);

        void AddSource(string source);
    }

    public interface IBlock
    {
        // TODO コンテキストにより異なるパターンあり、配列
        bool MaybeDynamic { get; }

        void Accept(IMethodBuilder builder);
    }

    public class SqlBlock : IBlock
    {
        public bool MaybeDynamic => false;

        public string Sql { get; }

        public SqlBlock(string sql)
        {
            Sql = sql;
        }

        public void Accept(IMethodBuilder builder) => builder.AddSql(Sql);
    }

    public class SourceBlock : IBlock
    {
        public bool MaybeDynamic => true;

        public string Source { get; }

        public SourceBlock(string source)
        {
            Source = source;
        }

        public void Accept(IMethodBuilder builder) => builder.AddSql(Source);
    }


    // TODO ブロックは情報だけ保持する形にするか コードの作りは呼び出し側で

    //public class SqlNode : INode
    //{
    //    public void Build(IBuildContext context)
    //        context.AddSource($"text(\"{value}\");");

    //public class ReplaceNode : INode
    //    public void Build(IBuildContext context)
    //        context.AddSource($"text({value});");

    // TODO 配列だとDynamicか！、
    //public class ParameterNode : INode
    //    private readonly string value;
    //    public void Build(IBuildContext context)
    //    {
    //        context.AddParameter(value);
    //        context.AddSource($"param({value});");
    //        context.AddSource("\r\n");

    //        // TODO IE<>なら複数のコード、この時点で引数飲めたデータにアクセスできている必要あり
    //        // 動的変数の展開には対応しないよ
    //    }
    //}

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
}
