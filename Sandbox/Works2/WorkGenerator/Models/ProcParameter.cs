namespace WorkGenerator.Models
{
    using System.Data;

    using DataLibrary.Attributes;

    public class ProcParameter
    {
        [Direction(ParameterDirection.Input)]
        public string InParam { get; set; }

        [Direction(ParameterDirection.InputOutput)]
        public int? InOutParam { get; set; }

        [Direction(ParameterDirection.Output)]
        public int OutParam { get; set; }
    }
}
