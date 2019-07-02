namespace DataLibrary.Nodes
{
    public sealed class SqlNode : INode
    {
        public string Value { get; }

        public SqlNode(string value)
        {
            Value = value;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
