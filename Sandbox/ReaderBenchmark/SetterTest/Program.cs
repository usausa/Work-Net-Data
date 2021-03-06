﻿using System;
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
            var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.IntNullableValue)));
            //var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.IntValue)));
            //var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.MyEnumNullableValue)));
            //var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.MyEnumValue)));
            //var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.MyStructNullableValue)));
            //var setter = generator.CreateNullSetter<Target>(type.GetProperty(nameof(Target.MyStructValue)));
            setter(instance);
        }
    }

    public struct MyStruct
    {
    }

    public enum MyEnum
    {
    }

    public class Target
    {
        private string stringValue;
        private int? intNullableValue;
        private int intValue;
        private MyEnum? myEnumNullableValue;
        private MyEnum myEnumValue;
        private MyStruct? myStructNullableValue;
        private MyStruct myStructValue;

        public string StringValue
        {
            get => stringValue;
            set => stringValue = value;
        }

        public int? IntNullableValue
        {
            get => intNullableValue;
            set => intNullableValue = value;
        }

        public int IntValue
        {
            get => intValue;
            set => intValue = value;
        }

        public MyEnum? MyEnumNullableValue
        {
            get => myEnumNullableValue;
            set => myEnumNullableValue = value;
        }

        public MyEnum MyEnumValue
        {
            get => myEnumValue;
            set => myEnumValue = value;
        }

        public MyStruct? MyStructNullableValue
        {
            get => myStructNullableValue;
            set => myStructNullableValue = value;
        }

        public MyStruct MyStructValue
        {
            get => myStructValue;
            set => myStructValue = value;
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
                if (LdcDictionary.TryGetValue(pi.PropertyType.IsEnum  ? pi.PropertyType.GetEnumUnderlyingType() : pi.PropertyType, out var action))
                {
                    action(il);
                }
                else
                {
                    var local = il.DeclareLocal(pi.PropertyType);

                    il.Emit(OpCodes.Ldloca_S, local);
                    il.Emit(OpCodes.Initobj, pi.PropertyType);
                    il.Emit(OpCodes.Ldloc_S, local);
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
