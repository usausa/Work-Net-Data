using System;
using System.Data;

namespace ReaderTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            var b = typeof(int?).IsValueType;
        }
    }

    public struct MyStruct
    {
    }

    public struct MyStruct2
    {
        public int A;

        public MyStruct2(int a)
        {
            A = a;
        }
    }

    public static class Helper
    {
        public static MyStruct GetMyStruct() => default(MyStruct);

        public static MyStruct2 GetMyStruct2() => default(MyStruct2);

        public static T GetDefault<T>() where T : struct
        {
            return default(T);
        }

        public static int GetInt() => default(int);
    }


    public class Data
    {
        public int StructValue { get; set; }

        public int? NullableStructValue { get; set; }

        public string ClassValue { get; set; }

        public string ConvertValue { get; set; }
    }

    public sealed class Mapper
    {
        public Func<object, object> converter;

        public Data Test()
        {
            var data = new Data();

            data.NullableStructValue = 1;

            return data;
        }

        public Data Map(IDataReader reader)
        {
            var data = new Data();

            var value = reader.GetValue(0);
            if (value is DBNull)
            {
                data.StructValue = default(int);
            }
            else
            {
                data.StructValue = (int)value;
            }

            value = reader.GetValue(1);
            if (value is DBNull)
            {
                data.NullableStructValue = null;
            }
            else
            {
                data.NullableStructValue = (int)value;
            }

            value = reader.GetValue(2);
            if (value is DBNull)
            {
                data.ClassValue = null;
            }
            else
            {
                data.ClassValue = (string)value;
            }

            value = reader.GetValue(3);
            if (value is DBNull)
            {
                data.ConvertValue = null;
            }
            else
            {
                data.ConvertValue = (string)converter(value);
            }

            return data;
        }
    }
}
