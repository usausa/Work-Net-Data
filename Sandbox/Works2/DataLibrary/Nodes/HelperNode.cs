namespace DataLibrary.Nodes
{
    public sealed class HelperNode : INode
    {
        public string Value { get; }

        public HelperNode(string value)
        {
            Value = value;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
