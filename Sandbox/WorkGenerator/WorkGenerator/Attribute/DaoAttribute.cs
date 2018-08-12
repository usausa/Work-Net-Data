namespace WorkGenerator.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class DaoAttribute : Attribute
    {
        public string DataSource { get; }

        public DaoAttribute()
            : this(string.Empty)
        {
        }

        public DaoAttribute(string dataSource)
        {
            DataSource = dataSource;
        }
    }
}
