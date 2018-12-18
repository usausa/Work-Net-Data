namespace WorkGenerator
{
    using WorkGenerator.Attribute;
    using WorkGenerator.Entity;

    [Dao]
    public interface ISampleDao
    {
        [Query("QueryHoge.sql")]
        HogeEntity QueryHoge(int id);

        [Execute("UpdateHoge.sql")]
        int UpdateHoge(HogeEntity entity, [Timeout] int timeout);
    }
}
