namespace Smart.Data.Accessor.Nodes
{
    using System.Collections.Generic;

    public abstract class NodeVisitorBase : INodeVisitor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public void Visit(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                node.Visit(this);
            }
        }

        public virtual void Visit(UsingNode node)
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
