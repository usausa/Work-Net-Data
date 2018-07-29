namespace WorkTokenizer.Tests
{
    using Xunit;

    public class SimpleTokenizerTest
    {
        //--------------------------------------------------------------------------------
        // Basic
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestBasic()
        {
            var tokenizer = new SimpleTokenizer("SELECT * FROM User WHERE Id = /*@ id */ 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(9, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("SELECT", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("*", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("FROM", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("User", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal("WHERE", tokens[4].Value);
            Assert.Equal(TokenType.Block, tokens[5].TokenType);
            Assert.Equal("Id", tokens[5].Value);
            Assert.Equal(TokenType.Block, tokens[6].TokenType);
            Assert.Equal("=", tokens[6].Value);
            Assert.Equal(TokenType.ParameterComment, tokens[7].TokenType);
            Assert.Equal("id", tokens[7].Value);
            Assert.Equal(TokenType.Block, tokens[8].TokenType);
            Assert.Equal("1", tokens[8].Value);
        }

        [Fact]
        public void TestBasic2()
        {
            var tokenizer = new SimpleTokenizer("SELECT * FROM User WHERE Id=/*@id*/1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(8, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("SELECT", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("*", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("FROM", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("User", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal("WHERE", tokens[4].Value);
            Assert.Equal(TokenType.Block, tokens[5].TokenType);
            Assert.Equal("Id=", tokens[5].Value);
            Assert.Equal(TokenType.ParameterComment, tokens[6].TokenType);
            Assert.Equal("id", tokens[6].Value);
            Assert.Equal(TokenType.Block, tokens[7].TokenType);
            Assert.Equal("1", tokens[7].Value);
        }

        //--------------------------------------------------------------------------------
        // IN
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestIn()
        {
            var tokenizer = new SimpleTokenizer("IN /*@ ids */ ('1', '2')");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(7, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("IN", tokens[0].Value);
            Assert.Equal(TokenType.ParameterComment, tokens[1].TokenType);
            Assert.Equal("ids", tokens[1].Value);
            Assert.Equal(TokenType.OpenParenthesis, tokens[2].TokenType);
            Assert.Equal("(", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("'1'", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal(",", tokens[4].Value);
            Assert.Equal(TokenType.Block, tokens[5].TokenType);
            Assert.Equal("'2'", tokens[5].Value);
            Assert.Equal(TokenType.CloseParenthesis, tokens[6].TokenType);
            Assert.Equal(")", tokens[6].Value);
        }

        //--------------------------------------------------------------------------------
        // Replace
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestReplace()
        {
            var tokenizer = new SimpleTokenizer("ORDER BY /*# column */");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("ORDER", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("BY", tokens[1].Value);
            Assert.Equal(TokenType.ReplaceComment, tokens[2].TokenType);
            Assert.Equal("column", tokens[2].Value);
        }

        //--------------------------------------------------------------------------------
        // Code
        //--------------------------------------------------------------------------------

        // TODO pragma, code

        //--------------------------------------------------------------------------------
        // Quate
        //--------------------------------------------------------------------------------

        // TODO

        //--------------------------------------------------------------------------------
        // EOL
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestEol()
        {
            var tokenizer = new SimpleTokenizer("SELECT\r\n  *\r\nFROM\r\n  User\r\n");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("SELECT", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("*", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("FROM", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("User", tokens[3].Value);
        }

        [Fact]
        public void TestEol2()
        {
            var tokenizer = new SimpleTokenizer("SELECT\r  *\rFROM\r  User\r");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("SELECT", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("*", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("FROM", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("User", tokens[3].Value);
        }

        [Fact]
        public void TestEol3()
        {
            var tokenizer = new SimpleTokenizer("SELECT\n  *\nFROM\n  User\n");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("SELECT", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("*", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("FROM", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("User", tokens[3].Value);
        }

        // TODO single \r \n ?

        //--------------------------------------------------------------------------------
        // Comment
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestComment()
        {
            var tokenizer = new SimpleTokenizer("WHERE /* comment */ Id = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Comment, tokens[1].TokenType);
            Assert.Equal("/* comment */", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("Id", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("=", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal("1", tokens[4].Value);
        }

        [Fact]
        public void TestEmptyComment()
        {
            var tokenizer = new SimpleTokenizer("WHERE /**/ Id = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Comment, tokens[1].TokenType);
            Assert.Equal("/**/", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("Id", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("=", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal("1", tokens[4].Value);
        }

        [Fact]
        public void TestCommentNotClosed()
        {
            var tokenizer = new SimpleTokenizer("WHERE /* comment");
            Assert.Throws<TokenizerException>(() => tokenizer.Tokenize());
        }

        //--------------------------------------------------------------------------------
        // Line comment
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestLineComment()
        {
            var tokenizer = new SimpleTokenizer("WHERE--comment\r\nId = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("Id", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("=", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("1", tokens[3].Value);
        }

        [Fact]
        public void TestLastLineComment2()
        {
            var tokenizer = new SimpleTokenizer("WHERE--comment\rId = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("Id", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("=", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("1", tokens[3].Value);
        }

        [Fact]
        public void TestLastLineComment3()
        {
            var tokenizer = new SimpleTokenizer("WHERE--comment\nId = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("Id", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("=", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("1", tokens[3].Value);
        }

        [Fact]
        public void TestLineCommentLast()
        {
            var tokenizer = new SimpleTokenizer("WHERE Id = 1--comment");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("Id", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("=", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("1", tokens[3].Value);
        }
    }
}
