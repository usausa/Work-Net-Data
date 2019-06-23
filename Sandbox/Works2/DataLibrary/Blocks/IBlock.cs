namespace DataLibrary.Blocks
{
    public interface IBlock
    {
        bool IsDynamic { get; }

        void Process(IBlockContext context);
    }
}
