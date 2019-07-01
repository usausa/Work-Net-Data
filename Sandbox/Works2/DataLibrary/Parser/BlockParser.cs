namespace DataLibrary.Parser
{
    using System.Collections.Generic;

    using DataLibrary.Fragments;
    using DataLibrary.Tokenizer;

    public sealed class BlockParser
    {
        private readonly IReadOnlyList<Token> tokens;

        private readonly List<IFragment> fragments = new List<IFragment>();

        public BlockParser(IReadOnlyList<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IReadOnlyList<IFragment> Parse()
        {
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Block:
                        fragments.Add(new SqlFragment(token.Value.Trim() + " "));
                        break;
                    case TokenType.OpenParenthesis:
                        fragments.Add(new SqlFragment(token.Value.Trim()));
                        break;
                    case TokenType.CloseParenthesis:
                        fragments.Add(new SqlFragment(token.Value.Trim() + " "));
                        break;
                    case TokenType.Comment:
                        ParseComment(token.Value.Trim());
                        break;
                }
            }

            return fragments;
        }

        private void ParseComment(string value)
        {
            if (value.StartsWith("!"))
            {
                fragments.Add(new HelperBlock(value.Substring(1).Trim()));
            }

            if (value.StartsWith("%"))
            {
                fragments.Add(new CodeBlock(value.Substring(1).Trim()));
            }

            if (value.StartsWith("#"))
            {
                fragments.Add(new RawSqlFragment(value.Substring(1).Trim()));
            }

            if (value.StartsWith("@"))
            {
                fragments.Add(new ParameterBlock(value.Substring(1).Trim()));
            }
        }
    }
}
