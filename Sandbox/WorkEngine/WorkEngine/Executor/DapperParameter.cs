namespace WorkEngine.Executor
{
    using Dapper;

    public sealed class DapperParameter : IParameter
    {
        private readonly DynamicParameters parameters = new DynamicParameters();

        public object Value => parameters;

        public void Add(string name, object value)
        {
            parameters.Add(name, value);
        }
    }
}
