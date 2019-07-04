namespace WorkGenerator.Models
{
    using DataLibrary.Attributes;

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
}
