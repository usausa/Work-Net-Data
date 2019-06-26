namespace DataLibrary.Generator
{
    using System;
    using System.Text;

    public class CodeBuilder
    {
        private readonly StringBuilder source = new StringBuilder();

        private bool newLine = true;

        public int Indent { get; set; }

        public CodeBuilder Write(string code)
        {
            WriteIndent();
            source.Append(code);
            return this;
        }

        public CodeBuilder WriteLine(string code)
        {
            WriteIndent();
            source.AppendLine(code);
            newLine = true;
            return this;
        }

        private void WriteIndent()
        {
            if (newLine)
            {
                source.Append(new String(' ', Indent * 4));
                newLine = false;
            }
        }

        public string ToSource() => source.ToString();
    }
}
