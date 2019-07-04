namespace WorkParser
{
    using System.Collections.Generic;

    using DataLibrary.Attributes;

    public class DataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class Parameter
    {
        public int InParam { get; set; }

        [InputOutput] public int InOutParam { get; set; }

        [Output] public int OutParam { get; set; }

        [ReturnValue] public int RetParam { get; set; }
    }

    [Dao]
    public interface ISampleDao
    {
        [ExecuteScalar]
        long Count();

        [ExecuteScalar]
        long Count(string code);

        IList<DataEntity> QueryDataList(string name, string code, string sort);

        [Insert("Data")]
        void Insert(DataEntity entity);

        [Procedure("PROC1")]
        void Call1(Parameter parameter);

        [Procedure("PROC2")]
        int Call2(int param1, ref int param2, out int param3);
    }
}
