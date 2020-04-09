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

    public enum MyEnum
    {
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

    public struct Data2
    {
        public int StructValue { get; set; }

        public int? NullableStructValue { get; set; }

        public string ClassValue { get; set; }

        public string ConvertValue { get; set; }
    }

    public class Data3
    {
        public int StructValue { get; set; }

        public int? NullableStructValue { get; set; }

        public string ClassValue { get; set; }

        public int ConvertStructValue { get; set; }

        public string ConvertClassValue { get; set; }

        public Data3(int structValue, int? nullableStructValue, string classValue, int convertStructValue, string convertClassValue)
        {
            StructValue = structValue;
            NullableStructValue = nullableStructValue;
            ClassValue = classValue;
            ConvertStructValue = convertStructValue;
            ConvertClassValue = convertClassValue;
        }
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

        public Data2 Map2(IDataReader reader)
        {
            var data = new Data2();

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


    public class Target
    {
        public string StringValue { get; set; }
        public int? IntNullableValue { get; set; }
        public int IntValue { get; set; }
        public MyStruct StructValue { get; set; }
        public MyEnum EnumValue { get; set; }
        public MyStruct? NullableStructValue { get; set; }
        public MyEnum? NullableEnumValue { get; set; }
    }

    public static class TargetNull
    {
        public static Target NullableValue()
        {
            var o = new Target();
            o.IntNullableValue = 0;
            o.NullableStructValue = new MyStruct();
            o.NullableEnumValue = 0;

            return o;
        }

        public static Target Mix()
        {
            var o = new Target();
            o.StringValue = null;
            o.IntNullableValue = null;
            o.IntValue = default;
            o.StructValue = default;
            o.NullableStructValue = default;
            o.EnumValue = default;
            o.NullableEnumValue = default;

            return o;
        }

        public static Target String()
        {
            var o = new Target();
            o.StringValue = null;
            o.StringValue = null;
            o.StringValue = null;
            return o;
        }

        public static Target IntNullable()
        {
            var o = new Target();
            o.IntNullableValue = null;
            o.IntNullableValue = null;
            o.IntNullableValue = null;
            return o;
        }

        public static Target Int()
        {
            var o = new Target();
            o.IntValue = default;
            o.IntValue = default;
            o.IntValue = default;
            return o;
        }

        public static Target Struct()
        {
            var o = new Target();
            o.StructValue = default;
            o.StructValue = default;
            o.StructValue = default;
            return o;
        }

        public static Target NullableStruct()
        {
            var o = new Target();
            o.NullableStructValue = default;
            o.NullableStructValue = default;
            o.NullableStructValue = default;
            return o;
        }

        public static Target Enum()
        {
            var o = new Target();
            o.EnumValue = default;
            o.EnumValue = default;
            o.EnumValue = default;
            return o;
        }

        public static Target NullableEnum()
        {
            var o = new Target();
            o.NullableEnumValue = default;
            o.NullableEnumValue = default;
            o.NullableEnumValue = default;
            return o;
        }
    }

    public sealed class ConverterCall
    {
        public static int ObjectToObjectInt(object value, Func<object, object> converter)
        {
            return (int)converter(value);
        }

        public static string ObjectToObjectString(object value, Func<object, object> converter)
        {
            return (string)converter(value);
        }

        public static int ObjectToInt(object value, Func<object, int> converter)
        {
            return converter(value);
        }

        public static string ObjectToString(object value, Func<object, string> converter)
        {
            return converter(value);
        }

        public static int TypedInt(object value, Func<int, int> converter)
        {
            return converter((int)value);
        }

        public static string TypedString(object value, Func<string, string> converter)
        {
            return converter((string)value);
        }
    }
}
