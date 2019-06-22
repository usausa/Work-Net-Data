namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Tokenizer;

    public abstract class LoaderMethodAttribute : MethodAttribute
    {
        private string value;

        protected LoaderMethodAttribute(MethodType methodType, string value)
            : base(CommandType.Text, methodType)
        {
            this.value = value;
        }

        public override ICollection<Token> CreateTokens(ISqlLoader loader, MethodInfo mi)
        {
            throw new System.NotImplementedException();
        }
    }
}
