using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using DataLibrary.Attributes;
using WorkGenerator.Models;

namespace WorkGenerator.Dao
{
    public class QueryParameter
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Sort { get; set; }
    }


    [Dao]
    public interface IMiscDao
    {
        [ExecuteScalar]
        long Count(string code);

        [ExecuteScalar]
        long Count2(string code);

        [Execute]
        int ExecuteIn1(string[] ids);

        [Execute]
        int ExecuteIn2(IList<string> ids);

        [Execute]
        int ExecuteIn3(IEnumerable<string> ids);

        [Query]
        IList<DataEntity> QueryDataList(string name, string code, string sort);

        [Query]
        IList<DataEntity> QueryDataList2(QueryParameter parameter);

        [Insert("Data")]
        void Insert(DataEntity entity);

        //[Procedure("PROC1")]
        //void Call1(Parameter parameter);

        [Procedure("PROC2")]
        int Call2(out int ret, int param1, ref int param2, out int param3);
    }
}
