using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibrary.Nodes;
using DataLibrary.Parser;
using DataLibrary.Tokenizer;

namespace WorkParser
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // TODO 0 class
            TestCount();
        }

        private static void TestCount()
        {
            var tokenizer = new SqlTokenizer("SELECT COUNT(*) FROM Data");
            var tokens = tokenizer.Tokenize();

            var parser = new NodeParser(tokens);
            var nodes = parser.Parse();

            // TODO array parameter: 1
            //var isStatic = blocks.All(x => !x.IsDynamic);
            //if (isStatic)
            //{
            //    var context = new StaticBlockContext();
            //    foreach (var block in blocks)
            //    {
            //        block.Process(context);
            //    }

            //    // TODO sql
            //    // TODO parameter
            //}

            // TODO parse

            // TODO helperがあるから、全メソッドを最初に処理か？: 2
        }

        // TODO 分離
    }

    //// TODO inner class & share context ?
    //public class BlockContext : IFragmentContext
    //{
    //    public bool IsDynamicParameter(string name)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class StaticCodeBuilder : IFragmentCodeBuilder
    //{
    //    private readonly StringBuilder sql = new StringBuilder();

    //    public void AddHelper(string value)
    //    {
    //    }

    //    public void AddSql(string value)
    //    {
    //        sql.Append(value).Append(" ");
    //    }

    //    public void AddRawSql(string value) => throw new NotSupportedException();

    //    public void AddCode(string value) => throw new NotSupportedException();

    //    public void AddParameter(string value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    // TODO Flush
    //}

    //public class DynamicCodeBuilder : IFragmentCodeBuilder
    //{
    //    private readonly List<string> helpers = new List<string>();

    //    public void AddHelper(string value)
    //    {
    //        helpers.Add(value);
    //    }

    //    public void AddSql(string value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddRawSql(string value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddCode(string value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddParameter(string value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    // TODO Flush
    //}
}
