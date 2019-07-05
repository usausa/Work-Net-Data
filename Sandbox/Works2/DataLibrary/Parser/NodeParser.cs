namespace DataLibrary.Parser
{
    using System.Collections.Generic;

    using DataLibrary.Tokenizer;
    using DataLibrary.Nodes;

    public sealed class NodeParser
    {
        private readonly IReadOnlyList<Token> tokens;

        private readonly List<INode> nodes = new List<INode>();

        private int current;

        private bool appendBlank;

        public NodeParser(IReadOnlyList<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Token NextToken() => current + 1 < tokens.Count ? tokens[current + 1] : null;

        private string AppendBlank(string value)
        {
            var left = appendBlank ? " " : string.Empty;
            appendBlank = false;
            var next = NextToken();
            var right = ((next != null) && (next.TokenType != TokenType.CloseParenthesis)) ? " " : string.Empty;
            return left + value + right;
        }

        public IReadOnlyList<INode> Parse()
        {
            while (current < tokens.Count)
            {
                var token = tokens[current];
                switch (token.TokenType)
                {
                    case TokenType.Block:
                        nodes.Add(new SqlNode(AppendBlank(token.Value.Trim())));
                        break;
                    case TokenType.OpenParenthesis:
                        nodes.Add(new SqlNode(token.Value.Trim()));
                        break;
                    case TokenType.CloseParenthesis:
                        nodes.Add(new SqlNode(AppendBlank(token.Value.Trim())));
                        break;
                    case TokenType.Comment:
                        ParseComment(token.Value.Trim());
                        break;
                }

                current++;
            }

            return nodes;
        }

        private void ParseComment(string value)
        {
            // Pragma
            if (value.StartsWith("!helper"))
            {
                nodes.Add(new UsingNode(true, value.Substring(7).Trim()));
            }

            if (value.StartsWith("!using"))
            {
                nodes.Add(new UsingNode(false, value.Substring(6).Trim()));
            }

            // Code
            if (value.StartsWith("%"))
            {
                nodes.Add(new CodeNode(value.Substring(1).Trim()));
            }

            // Raw
            if (value.StartsWith("#"))
            {
                nodes.Add(new RawSqlNode(value.Substring(1).Trim()));
            }

            // Parameter
            if (value.StartsWith("@"))
            {
                nodes.Add(new ParameterNode(value.Substring(1).Trim()));

                appendBlank = true;
                var next = NextToken();
                if (next != null)
                {
                    current++;

                    if (next.TokenType == TokenType.OpenParenthesis)
                    {
                        var count = 1;
                        while ((count > 0) || (current < tokens.Count))
                        {
                            var token = tokens[current];
                            if (token.TokenType == TokenType.OpenParenthesis)
                            {
                                count++;
                            }
                            else if (token.TokenType == TokenType.CloseParenthesis)
                            {
                                count--;
                            }

                            current++;
                        }
                    }
                }
            }
        }
    }
}
