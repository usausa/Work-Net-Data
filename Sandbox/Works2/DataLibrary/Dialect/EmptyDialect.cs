namespace DataLibrary.Dialect
{
    public sealed class EmptyDialect : IEmptyDialect
    {
        public string GetSql() => "SELECT NULL WHERE 1 = 0";
    }
}
