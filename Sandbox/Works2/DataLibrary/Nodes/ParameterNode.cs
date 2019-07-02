namespace DataLibrary.Nodes
{
    public sealed class ParameterNode : INode
    {
        public string Value { get; }

        public ParameterNode(string value)
        {
            Value = value;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
