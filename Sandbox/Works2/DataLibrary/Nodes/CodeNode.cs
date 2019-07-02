namespace DataLibrary.Nodes
{
    public sealed class CodeNode : INode
    {
        public string Value { get; }

        public CodeNode(string value)
        {
            Value = value;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
