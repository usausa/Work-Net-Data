namespace DataLibrary.Nodes
{
    public interface INodeVisitor
    {
        void Visit(HelperNode node);

        void Visit(SqlNode node);

        void Visit(RawSqlNode node);

        void Visit(CodeNode node);

        void Visit(ParameterNode node);
    }
}
