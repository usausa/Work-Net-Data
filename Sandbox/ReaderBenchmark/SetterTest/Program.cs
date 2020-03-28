using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Smart;

namespace SetterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new Target();

            var type = typeof(Target);
            var generator = new Generator();

            //var stringSetter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.StringValue)));
            //stringSetter(instance);

            //var intNullableSetter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.IntNullableValue)));
            //intNullableSetter(instance);

            var intSetter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.IntValue)));
            intSetter(instance);
        }
    }

    public class Target
    {
        private string stringValue;
        public string StringValue
        {
            get => stringValue;
            set => stringValue = value;
        }

        private int? intNullableValue;
        public int? IntNullableValue
        {
            get => intNullableValue;
            set => intNullableValue = value;
        }

        private int intValue;
        public int IntValue
        {
            get => intValue;
            set => intValue = value;
        }
    }

    public class Generator
    {
        private AssemblyBuilder assemblyBuilder;

        private ModuleBuilder moduleBuilder;

        private ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
                {
                    assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName("NewResultMapperFactoryAssembly"),
                        AssemblyBuilderAccess.Run);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(
                        "NewResultMapperFactoryModule");
                }

                return moduleBuilder;
            }
        }

        public Action<T> CreateNullSetter<T>(PropertyInfo pi)
        {
            var dynamicMethod = new DynamicMethod(
                string.Empty,
                typeof(void),
                new[] { typeof(object), pi.DeclaringType },
                true);
            var il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1);

            if (pi.PropertyType.IsValueType)
            {
                if (pi.PropertyType.IsNullableType())
                {
                    // TODO これで良いか？
                    il.Emit(OpCodes.Ldnull);
                }
                if (LdcDictionary.TryGetValue(pi.PropertyType.IsEnum  ? pi.PropertyType.GetEnumUnderlyingType() : pi.PropertyType, out var action))
                {
                    action(il);
                }
                else
                {

                }

                il.Emit(OpCodes.Callvirt, pi.GetSetMethod());
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Callvirt, pi.GetSetMethod());
            }

            il.Emit(OpCodes.Ret);

            var delegateType = typeof(Action<>).MakeGenericType(pi.DeclaringType);
            return (Action<T>)dynamicMethod.CreateDelegate(delegateType, null);
        }

        private static readonly Dictionary<Type, Action<ILGenerator>> LdcDictionary = new Dictionary<Type, Action<ILGenerator>>
        {
            { typeof(bool), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(byte), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(char), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(short), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(int), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(sbyte), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(ushort), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(uint), il => il.Emit(OpCodes.Ldc_I4_0) },      // Simplicity
            { typeof(long), il => il.Emit(OpCodes.Ldc_I8, 0L) },
            { typeof(ulong), il => il.Emit(OpCodes.Ldc_I8, 0L) },   // Simplicity
            { typeof(float), il => il.Emit(OpCodes.Ldc_R4, 0f) },
            { typeof(double), il => il.Emit(OpCodes.Ldc_R8, 0d) },
            { typeof(IntPtr), il => il.Emit(OpCodes.Ldc_I4_0) },    // Simplicity
            { typeof(UIntPtr), il => il.Emit(OpCodes.Ldc_I4_0) },   // Simplicity
        };
    }
}
