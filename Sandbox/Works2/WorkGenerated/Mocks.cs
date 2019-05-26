namespace WorkGenerated
{
    using System.Data.Common;

    public interface IDbProvider
    {
        DbConnection CreateConnection();
    }

    public class DataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
