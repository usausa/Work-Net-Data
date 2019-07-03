using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using DataLibrary.Attributes;
using WorkGenerator.Models;

namespace WorkGenerator.Dao
{
    [Dao]
    public interface IMiscDao
    {
        [ExecuteScalar]
        long Count(string code);

        [ExecuteScalar]
        long Count2(string code);

        [Query]
        IList<DataEntity> QueryDataList(string name, string code, string sort);

        [Insert("Data")]
        void Insert(DataEntity entity);

        //[Procedure("PROC1")]
        //void Call1(Parameter parameter);

        //[Procedure("PROC2")]
        //int Call2(int param1, ref int param2, out int param3);
    }
}
