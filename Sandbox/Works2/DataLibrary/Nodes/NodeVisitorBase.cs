namespace DataLibrary.Nodes
{
    public abstract class NodeVisitorBase : INodeVisitor
    {
        public virtual void Visit(HelperNode node)
        {
        }

        public virtual void Visit(SqlNode node)
        {
        }

        public virtual void Visit(RawSqlNode node)
        {
        }

        public virtual void Visit(CodeNode node)
        {
        }

        public virtual void Visit(ParameterNode node)
        {
        }
    }
}
