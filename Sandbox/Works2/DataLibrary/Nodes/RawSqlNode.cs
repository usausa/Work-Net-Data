namespace DataLibrary.Nodes
{
    public sealed class RawSqlNode : INode
    {
        public string Value { get; }

        public RawSqlNode(string value)
        {
            Value = value;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
