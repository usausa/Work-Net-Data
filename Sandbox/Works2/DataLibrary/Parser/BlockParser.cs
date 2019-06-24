namespace DataLibrary.Parser
{
    using System.Collections.Generic;

    using DataLibrary.Blocks;
    using DataLibrary.Tokenizer;

    public sealed class BlockParser
    {
        private readonly IReadOnlyList<Token> tokens;

        private readonly List<IBlock> blocks = new List<IBlock>();

        public BlockParser(IReadOnlyList<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IReadOnlyList<IBlock> Parse()
        {
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Block:
                        blocks.Add(new SqlBlock(token.Value.Trim() + " "));
                        break;
                    case TokenType.OpenParenthesis:
                        blocks.Add(new SqlBlock(token.Value.Trim()));
                        break;
                    case TokenType.CloseParenthesis:
                        blocks.Add(new SqlBlock(token.Value.Trim() + " "));
                        break;
                    case TokenType.Comment:
                        ParseComment(token.Value.Trim());
                        break;
                }
            }

            return blocks;
        }

        private void ParseComment(string value)
        {
            if (value.StartsWith("!"))
            {
                blocks.Add(new ImportBlock(value.Substring(1).Trim(), false));
            }

            if (value.StartsWith("$"))
            {
                blocks.Add(new ImportBlock(value.Substring(1).Trim(), true));
            }

            if (value.StartsWith("%"))
            {
                blocks.Add(new CodeBlock(value.Substring(1).Trim()));
            }

            if (value.StartsWith("#"))
            {
                blocks.Add(new RawSqlBlock(value.Substring(1).Trim()));
            }

            if (value.StartsWith("@"))
            {
                blocks.Add(new ParameterBlock(value.Substring(1).Trim()));
            }
        }
    }
}
