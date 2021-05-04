using System;
using System.Collections.Generic;

namespace WorkNullableAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO enableありのメタデータ確認
            // TODO e -> d, d ->e の警告確認、戻り値？
            // TODO ビルドオプション？、MSBuildなので別口？
        }
    }

    //--------------------------------------------------------------------------------
    // Interface
    //--------------------------------------------------------------------------------
#nullable enable
    public interface INullableAccessor
    {
        T? QueryFirstOrDefault<T>();

        List<T> QueryList<T>();

        string? QuerySingle();

        string QuerySingleNonNullable();

        int Execute(string? param);

        int ExecuteNonNullable(string param);
    }
#nullable restore

#nullable disable
    public interface INonNullableAccessor
    {
        T QueryFirstOrDefault<T>();

        List<T> QueryList<T>();

        string QuerySingle();

        int Execute(string param);
    }
#nullable restore
}
