namespace LexerTest
{
    using System;
    using System.Collections.Generic;

    public enum TokenType
    {
        Comment,
        Block
    }

    public class Token
    {
        public TokenType TokenType { get; }

        public string Value { get; }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
    }

    public class SimpleTokenizer
    {
        private readonly List<Token> tokens = new List<Token>();

        private readonly string source;

        private int current;

        public SimpleTokenizer(string source)
        {
            this.source = source;
        }

        public IList<Token> Tokenize()
        {
            int remain;
            while ((remain = source.Length - current) > 0)
            {
                if (remain >= 2)
                {
                    Peek2Chars();
                }
                else if (remain >= 1)
                {
                    Peek1Chars();
                }
            }

            return tokens;
        }

        private void Peek2Chars()
        {
            if ((source[current] == '/') && (source[current + 1] == '*'))
            {
                // Block comment

                // TODO
                current += 2;
            }
            else if ((source[current] == '-') && (source[current + 1] == '-'))
            {
                // Line comment

                // TODO
                current += 2;
            }
            else if ((source[current] == '\r') && (source[current + 1] == '\n'))
            {
                // EOL

                // TODO

                current += 2;
            }
            else
            {
                Peek1Chars();
            }
        }

        private void Peek1Chars()
        {
            if (Char.IsWhiteSpace(source[current]))
            {
                // Space
                current += 1;
            }
            else if (source[current] == '\'')
            {
                // Quate

                // TODO
                current += 1;
            }
            else if ((source[current] == '\r') || (source[current] == '\n'))
            {
                // EOL

                // TODO
                current += 1;
            }
            else
            {
                // TODO Word
                current += 1;   // dummy
            }
        }
    }
}
