namespace LexerTest
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TokenizerException : Exception
    {
        public TokenizerException(string message)
            : base(message)
        {
        }
    }

    public enum TokenType
    {
        CodeComment,
        ParameterComment,
        ReplaceComment,
        Block,
        OpenParenthesis,
        CloseParenthesis
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
            if ((source[current] == '\r') && (source[current + 1] == '\n'))
            {
                // EOL
                current += 2;
            }
            else if ((source[current] == '-') && (source[current + 1] == '-'))
            {
                // Line comment
                current += 2;

                int remain;
                while ((remain = source.Length - current) > 0)
                {
                    if ((remain >= 2) && (source[current] == '\r') && (source[current + 1] == '\n'))
                    {
                        current += 2;
                        return;
                    }
                    if ((remain >= 1) && ((source[current] == '\r') || (source[current] == '\n')))
                    {
                        current += 1;
                        return;
                    }

                    current++;
                }
            }
            else if ((source[current] == '/') && (source[current + 1] == '*'))
            {
                // Block comment
                current += 2;

                var start = 0;
                var tokenType = default(TokenType?);
                if (current < source.Length)
                {
                    if (source[current] == '@')
                    {
                        tokenType = TokenType.ParameterComment;
                        current++;
                        start = current;
                    }
                    else if (source[current] == '#')
                    {
                        tokenType = TokenType.ReplaceComment;
                        current++;
                        start = current;
                    }
                    else if (source[current] == '%')
                    {
                        tokenType = TokenType.CodeComment;
                        current++;
                        start = current;
                    }
                }

                while (current < source.Length - 1)
                {
                    if ((source[current] == '*') && (source[current + 1] == '/'))
                    {
                        if (tokenType.HasValue)
                        {
                            tokens.Add(new Token(TokenType.Block, source.Substring(start, current - start).Trim()));
                        }

                        return;
                    }

                    current++;
                }

                throw new TokenizerException("Invalid sql. Comment is not closed.");
            }

            Peek1Chars();
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
                var start = current;
                current++;

                var closed = false;
                while (current < source.Length && !closed)
                {
                    if (source[current] == '\'')
                    {
                        current++;

                        if ((current < source.Length) && (source[current] == '\''))
                        {
                            current++;
                        }
                        else
                        {
                            closed = true;
                        }
                    }
                    else
                    {
                        current++;
                    }
                }

                if (!closed)
                {
                    throw new TokenizerException("Invalid sql. Quate is not closed.");
                }

                tokens.Add(new Token(TokenType.Block, source.Substring(start, current - start)));
            }
            else if (source[current] == '(')
            {
                // Open
                tokens.Add(new Token(TokenType.OpenParenthesis, "("));
                current += 1;
            }
            else if (source[current] == ')')
            {
                // Close
                tokens.Add(new Token(TokenType.CloseParenthesis, ")"));
                current += 1;
            }
            else if ((source[current] == '\r') || (source[current] == '\n'))
            {
                // EOL
                current += 1;
            }
            else
            {
                // Block
                var start = current;
                current++;

                while ((current < source.Length) && IsBlockChar(source[current]))
                {
                    current++;
                }

                tokens.Add(new Token(TokenType.Block, source.Substring(start, current - start)));
            }
        }

        private bool IsBlockChar(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                return false;
            }

            switch (c)
            {
                case '\'':
                case '(':
                case ')':
                case '\r':
                case '\n':
                    return false;
            }

            return true;
        }
    }
}
