namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Tokenizer;

    public sealed class InsertAttribute : MethodAttribute
    {
        public InsertAttribute()
            : base(CommandType.Text, MethodType.Execute)
        {
        }

        public override ICollection<Token> CreateTokens(ISqlLoader loader, MethodInfo mi)
        {
            throw new System.NotImplementedException();
        }
    }
}
