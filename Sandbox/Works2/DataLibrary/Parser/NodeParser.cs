namespace DataLibrary.Parser
{
    using System.Collections.Generic;

    using DataLibrary.Tokenizer;
    using DataLibrary.Nodes;

    public sealed class NodeParser
    {
        private readonly IReadOnlyList<Token> tokens;

        private readonly List<INode> nodes = new List<INode>();

        public NodeParser(IReadOnlyList<Token> tokens)
        {
            this.tokens = tokens;
        }

        public IReadOnlyList<INode> Parse()
        {
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Block:
                        nodes.Add(new SqlNode(token.Value.Trim() + " "));
                        break;
                    case TokenType.OpenParenthesis:
                        nodes.Add(new SqlNode(token.Value.Trim()));
                        break;
                    case TokenType.CloseParenthesis:
                        nodes.Add(new SqlNode(token.Value.Trim() + " "));
                        break;
                    case TokenType.Comment:
                        ParseComment(token.Value.Trim());
                        break;
                }
            }

            return nodes;
        }

        private void ParseComment(string value)
        {
            if (value.StartsWith("!"))
            {
                nodes.Add(new HelperNode(value.Substring(1).Trim()));
            }

            if (value.StartsWith("%"))
            {
                nodes.Add(new CodeNode(value.Substring(1).Trim()));
            }

            if (value.StartsWith("#"))
            {
                nodes.Add(new RawSqlNode(value.Substring(1).Trim()));
            }

            if (value.StartsWith("@"))
            {
                nodes.Add(new ParameterNode(value.Substring(1).Trim()));
            }
        }
    }
}
