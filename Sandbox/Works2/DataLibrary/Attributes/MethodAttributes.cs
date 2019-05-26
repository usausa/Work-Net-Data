using System.Data;

namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAttribute : Attribute
    {
        public CommandType CommandType { get; }

        // TODO メソッドタイプを返すか？

        // TODO Textをどうするか
        public string Text { get; }

        protected MethodAttribute(CommandType commandType, string text)
        {
            CommandType = commandType;
            Text = text;
        }
    }

    // TODO 基本属性もBuilderにする？

    //public sealed class ExecuteAttribute : MethodAttribute
    //{
    //}

    //public sealed class ExecuteScalarAttribute : MethodAttribute
    //{
    //}

    // TODO ExecuteReader CommandBehavior

    // TODO FirstOrDefault ?

    // TODO buffer default ? (false dapperとは違うが)  IListとIEで変更する手もある！
    //public sealed class QueryAttribute : MethodAttribute
    //{
    //}

    // TODO interface SQL Builder (in meta info out Block?)

    // TODO interface implemented
    //public sealed class ProcedureAttribute : CommandAttribute
    //{
    //    public ProcedureAttribute(string name) : base(CommandType.StoredProcedure, name)
    //    {
    //    }
    //}

    //public sealed class SqlAttribute : CommandAttribute
    //{
    //    public SqlAttribute(string sql, CommandType commandType = CommandType.Text) : base(commandType, sql)
    //    {
    //    }
    //}

    // TODO Insert

    // TODO Select parameter key

    // TODO Delete parameter key
}
