﻿namespace DataLibrary.Blocks
{
    public sealed class SqlBlock : IBlock
    {
        private readonly string value;

        public SqlBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IBlockContext context) => false;

        public void Build(ICodeBuilder builder) => builder.AddSql(value);
    }
}
