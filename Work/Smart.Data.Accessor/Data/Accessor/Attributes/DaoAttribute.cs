namespace Smart.Data.Accessor.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class DaoAttribute : Attribute
    {
        public string DataSource { get; }

        public DaoAttribute(string dataSource)
        {
            DataSource = dataSource;
        }
    }
}
