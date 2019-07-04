namespace DataLibrary.Attributes
{
    using System.Data;

    public sealed class DbTypeBuilderAttribute : ParameterBuilderAttribute
    {
        public DbTypeBuilderAttribute(DbType dbType)
            : base(dbType)
        {
        }

        public DbTypeBuilderAttribute(DbType dbType, int size)
            : base(dbType, size)
        {
        }
    }
}
