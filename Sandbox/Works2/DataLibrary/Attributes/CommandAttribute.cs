namespace DataLibrary.Attributes
{
    using System;
    using System.Data;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class CommandAttribute : Attribute
    {
        public CommandType CommandType { get; }

        public string Text { get; }

        protected CommandAttribute(CommandType commandType, string text)
        {
            CommandType = commandType;
            Text = text;
        }
    }

    public sealed class ProcedureAttribute : CommandAttribute
    {
        public ProcedureAttribute(string name) : base(CommandType.StoredProcedure, name)
        {
        }
    }

    public sealed class ResourceAttribute : CommandAttribute
    {
        public ResourceAttribute(string name, CommandType commandType = CommandType.Text) : base(commandType, name)
        {
        }
    }

    public sealed class SqlAttribute : CommandAttribute
    {
        public SqlAttribute(string sql, CommandType commandType = CommandType.Text) : base(commandType, sql)
        {
        }
    }
}
