namespace DataLibrary.Nodes
{
    public sealed class HelperNode : INode
    {
        public string FullName { get; }

        public HelperNode(string fullName)
        {
            FullName = fullName;
        }

        public void Visit(INodeVisitor visitor) => visitor.Visit(this);
    }
}
