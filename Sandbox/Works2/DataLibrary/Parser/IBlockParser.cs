using System.Collections.Generic;
namespace DataLibrary.Parser
{
    using DataLibrary.Blocks;
    using DataLibrary.Tokenizer;

    public interface IBlockParser
    {
        IReadOnlyList<IBlock> Parse(IReadOnlyList<Token> tokens);
    }
}
