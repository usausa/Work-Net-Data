namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Blocks;
    using DataLibrary.Loader;
    using DataLibrary.Parser;

    public abstract class LoaderMethodAttribute : MethodAttribute
    {
        protected LoaderMethodAttribute(MethodType methodType)
            : base(CommandType.Text, methodType)
        {
        }

        public override IReadOnlyList<IBlock> CreateTokens(ISqlLoader loader, IBlockParser parser, MethodInfo mi)
        {
            throw new System.NotImplementedException();
        }
    }
}
