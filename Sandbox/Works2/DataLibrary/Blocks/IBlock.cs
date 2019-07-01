namespace DataLibrary.Blocks
{
    using System.Reflection;

    public interface IBlock
    {
        bool IsDynamic(IBlockContext context);

        void Build(ICodeBuilder builder);
    }
}
