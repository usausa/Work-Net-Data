namespace DataLibrary.Nodes
{
    public interface INode
    {
        void Visit(INodeVisitor visitor);
    }
}
