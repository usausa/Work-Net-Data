namespace WorkPerformance
{
    using Smart.Data.Accessor.Attributes;

    public class DataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ProcParameter
    {
        [Input]
        public string InParam { get; set; }

        [InputOutput]
        public int? InOutParam { get; set; }

        [Output]
        public int OutParam { get; set; }

        [ReturnValue]
        public int Result { get; set; }
    }

    public class QueryParameter
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Order { get; set; }
    }
}
