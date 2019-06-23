namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Blocks;
    using DataLibrary.Loader;
    using DataLibrary.Parser;

    public sealed class InsertAttribute : MethodAttribute
    {
        public InsertAttribute()
            : base(CommandType.Text, MethodType.Execute)
        {
        }

        public override IReadOnlyList<IBlock> CreateTokens(ISqlLoader loader, IBlockParser parser, MethodInfo mi)
        {
            throw new System.NotImplementedException();
        }
    }
}
