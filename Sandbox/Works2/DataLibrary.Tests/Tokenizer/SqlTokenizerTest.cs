namespace DataLibrary.Tokenizer
{
    using Xunit;

    public class SqlTokenizerTest
    {
        //--------------------------------------------------------------------------------
        // Basic
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestBasic()
        {
            var tokenizer = new SqlTokenizer("SELECT * FROM User WHERE Id = /* @id */ 1");
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
            Assert.Equal(TokenType.Comment, tokens[7].TokenType);
            Assert.Equal("@id", tokens[7].Value);
            Assert.Equal(TokenType.Block, tokens[8].TokenType);
            Assert.Equal("1", tokens[8].Value);
        }

        [Fact]
        public void TestBasic2()
        {
            var tokenizer = new SqlTokenizer("SELECT * FROM User WHERE Id=/*@id*/1");
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
            Assert.Equal(TokenType.Comment, tokens[6].TokenType);
            Assert.Equal("@id", tokens[6].Value);
            Assert.Equal(TokenType.Block, tokens[7].TokenType);
            Assert.Equal("1", tokens[7].Value);
        }

        //--------------------------------------------------------------------------------
        // IN
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestIn()
        {
            var tokenizer = new SqlTokenizer("IN /* @ids */ ('1', '2')");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(7, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("IN", tokens[0].Value);
            Assert.Equal(TokenType.Comment, tokens[1].TokenType);
            Assert.Equal("@ids", tokens[1].Value);
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
        // Code
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestCode()
        {
            var tokenizer = new SqlTokenizer(
                "/*!using System */\r\n" +
                "SELECT * FROM Employee\r\n" +
                "/*% if (!String.IsNullOrEmpty(sort)) { */\r\n" +
                "ORDER BY /* #sort */ Id\r\n" +
                "/*% } */\r\n");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(11, tokens.Count);
            Assert.Equal(TokenType.Comment, tokens[0].TokenType);
            Assert.Equal("! System", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("SELECT", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("*", tokens[2].Value);
            Assert.Equal(TokenType.Block, tokens[3].TokenType);
            Assert.Equal("FROM", tokens[3].Value);
            Assert.Equal(TokenType.Block, tokens[4].TokenType);
            Assert.Equal("Employee", tokens[4].Value);
            Assert.Equal(TokenType.Comment, tokens[5].TokenType);
            Assert.Equal("% if (!String.IsNullOrEmpty(sort)) {", tokens[5].Value);
            Assert.Equal(TokenType.Block, tokens[6].TokenType);
            Assert.Equal("ORDER", tokens[6].Value);
            Assert.Equal(TokenType.Block, tokens[7].TokenType);
            Assert.Equal("BY", tokens[7].Value);
            Assert.Equal(TokenType.Comment, tokens[8].TokenType);
            Assert.Equal("#sort", tokens[8].Value);
            Assert.Equal(TokenType.Block, tokens[9].TokenType);
            Assert.Equal("Id", tokens[9].Value);
            Assert.Equal(TokenType.Comment, tokens[10].TokenType);
            Assert.Equal("% }", tokens[10].Value);
        }

        //--------------------------------------------------------------------------------
        // Quate
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestQuate()
        {
            var tokenizer = new SqlTokenizer("Name = 'abc'");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("Name", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("=", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("'abc'", tokens[2].Value);
        }

        [Fact]
        public void TestQuateEscaped()
        {
            var tokenizer = new SqlTokenizer("Name = 'abc'''");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("Name", tokens[0].Value);
            Assert.Equal(TokenType.Block, tokens[1].TokenType);
            Assert.Equal("=", tokens[1].Value);
            Assert.Equal(TokenType.Block, tokens[2].TokenType);
            Assert.Equal("'abc'''", tokens[2].Value);
        }


        [Fact]
        public void TestQuateNotClosed()
        {
            var tokenizer = new SqlTokenizer("Name = 'abc");
            Assert.Throws<SqlTokenizerException>(() => tokenizer.Tokenize());
        }

        [Fact]
        public void TestQuateEscapedNotClosed()
        {
            var tokenizer = new SqlTokenizer("Name = 'abc''xyz''");
            Assert.Throws<SqlTokenizerException>(() => tokenizer.Tokenize());
        }

        //--------------------------------------------------------------------------------
        // EOL
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestEol()
        {
            var tokenizer = new SqlTokenizer("SELECT\r\n  *\r\nFROM\r\n  User\r\n");
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
            var tokenizer = new SqlTokenizer("SELECT\r  *\rFROM\r  User\r");
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
            var tokenizer = new SqlTokenizer("SELECT\n  *\nFROM\n  User\n");
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

        //--------------------------------------------------------------------------------
        // Comment
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestComment()
        {
            var tokenizer = new SqlTokenizer("WHERE /* comment */ Id = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Comment, tokens[1].TokenType);
            Assert.Equal("comment", tokens[1].Value);
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
            var tokenizer = new SqlTokenizer("WHERE /**/ Id = 1");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.Block, tokens[0].TokenType);
            Assert.Equal("WHERE", tokens[0].Value);
            Assert.Equal(TokenType.Comment, tokens[1].TokenType);
            Assert.Equal("", tokens[1].Value);
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
            var tokenizer = new SqlTokenizer("WHERE /* comment");
            Assert.Throws<SqlTokenizerException>(() => tokenizer.Tokenize());
        }

        //--------------------------------------------------------------------------------
        // Line comment
        //--------------------------------------------------------------------------------

        [Fact]
        public void TestLineComment()
        {
            var tokenizer = new SqlTokenizer("WHERE--comment\r\nId = 1");
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
            var tokenizer = new SqlTokenizer("WHERE--comment\rId = 1");
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
            var tokenizer = new SqlTokenizer("WHERE--comment\nId = 1");
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
            var tokenizer = new SqlTokenizer("WHERE Id = 1--comment");
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
