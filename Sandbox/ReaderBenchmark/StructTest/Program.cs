using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Smart;
using Smart.Reflection.Emit;

namespace StructTest
{
    public class Class0
    {
        public int Value { get; set; }
    }

    public class Class1
    {
        public int Value { get; set; }

        public Class1(int value)
        {
            Debug.WriteLine("** Class1 ** " + value);
            Value = value;
        }
    }

    public struct Struct0
    {
        public int Value { get; set; }
    }

    public struct Struct1
    {
        public int Value { get; set; }

        public Struct1(int value)
        {
            Debug.WriteLine("** Struct1 ** " + value);
            Value = value;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var factory = new CreateAndSetFactory();

            Debug.WriteLine("----------");
            var actionC0 = factory.Create<Class0, int>(0, "Value");
            var objC0 = actionC0(123);
            Debug.WriteLine(objC0.Value);

            Debug.WriteLine("----------");
            var actionC1 = factory.Create<Class1, int>(1, "Value");
            var objC1 = actionC1(123);
            Debug.WriteLine(objC1.Value);

            Debug.WriteLine("----------");
            var actionS0 = factory.Create<Struct0, int>(0, "Value");
            var objS0 = actionS0(123);
            Debug.WriteLine(objS0.Value);

            Debug.WriteLine("----------");
            var actionS1 = factory.Create<Struct1, int>(1, "Value");
            var objS1 = actionS1(123);
            Debug.WriteLine(objS1.Value);

            Debug.WriteLine("----------");
            var actionS10 = factory.Create<Struct1, int>(0, "Value");
            var objS10 = actionS10(123);
            Debug.WriteLine(objS10.Value);

            Debug.WriteLine("----------");
            var actionS0N = factory.Create<Struct0?, int>(0, "Value");
            var objS0N = actionS0N(123);
            Debug.WriteLine(objS0N.Value.Value);

            Debug.WriteLine("----------");
            var actionS1N = factory.Create<Struct1?, int>(1, "Value");
            var objS1N = actionS1N(123);
            Debug.WriteLine(objS1N.Value.Value);

            Debug.WriteLine("----------");
            var actionS10N = factory.Create<Struct1?, int>(0, "Value");
            var objS10N = actionS10N(123);
            Debug.WriteLine(objS10N.Value.Value);
        }
    }

    public static class NullableFactory
    {
        public static Struct0? Create()
        {
            return null;
        }
    }

    public class CreateAndSetFactory
    {
        private int typeNo;

        private AssemblyBuilder assemblyBuilder;

        private ModuleBuilder moduleBuilder;

        private ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
                {
                    assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName("ObjectResultMapperFactoryAssembly"),
                        AssemblyBuilderAccess.Run);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(
                        "ObjectResultMapperFactoryModule");
                }

                return moduleBuilder;
            }
        }

        public Func<TProperty, T> Create<T, TProperty>(int ctorArgumentCount, string propertyName)
        {
            var type = typeof(T);
            var isNullableType = type.IsValueType && type.IsNullableType();
            var targetType = isNullableType ? Nullable.GetUnderlyingType(type) : type;
            var pi = targetType.GetProperty(propertyName);
            var ci = targetType.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == ctorArgumentCount);

            // Prepare
            var typeBuilder = ModuleBuilder.DefineType(
                $"Holder_{typeNo}",
                TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed |
                TypeAttributes.BeforeFieldInit);
            typeNo++;

            var typeInfo = typeBuilder.CreateTypeInfo();
            var holderType = typeInfo.AsType();
            var holder = Activator.CreateInstance(holderType);

            var dynamicMethod = new DynamicMethod(string.Empty, type, new[] { holderType, pi.PropertyType }, true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Variables
            // TODO nullable ?
            var ctorLocal = targetType.IsValueType ? ilGenerator.DeclareLocal(targetType) : null;
            var valueTypeLocals = ilGenerator.DeclareValueTypeLocals(
                (ci?.GetParameters().Select(x => x.ParameterType) ?? Array.Empty<Type>()).Append(pi.PropertyType));

            // IL
            if (ctorLocal != null)
            {
                ilGenerator.Emit(OpCodes.Ldloca, ctorLocal);
            }

            if (ci != null)
            {
                foreach (var pmi in ci.GetParameters())
                {
                    ilGenerator.Emit(OpCodes.Ldc_I4, 234);
                    //ilGenerator.EmitStackDefault(pmi.ParameterType, valueTypeLocals);
                }

                if (ctorLocal != null)
                {
                    // Struct call constructor
                    ilGenerator.Emit(OpCodes.Call, ci);
                }
                else
                {
                    // Class new
                    ilGenerator.Emit(OpCodes.Newobj, ci);
                }
            }
            else
            {
                // Struct init
                ilGenerator.Emit(OpCodes.Initobj, targetType);
            }

            if (ctorLocal != null)
            {
                ilGenerator.Emit(OpCodes.Ldloca, ctorLocal);
            }

            // Prop
            ilGenerator.Emit(OpCodes.Dup);

            ilGenerator.Emit(OpCodes.Ldarg_1); // stack argument
            ilGenerator.EmitSetter(pi);

            // Return
            if (ctorLocal != null)
            {
                ilGenerator.Emit(OpCodes.Ldobj, targetType);
            }

            if (isNullableType)
            {
                ilGenerator.EmitChangeToNullable(type);
            }

            ilGenerator.Emit(OpCodes.Ret);

            var funcType = typeof(Func<,>).MakeGenericType(typeof(TProperty), type);
            return (Func<TProperty, T>) dynamicMethod.CreateDelegate(funcType, holder);
        }
    }

        internal static class EmitExtensions
    {
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

        private static readonly MethodInfo GetValue = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue));

        public static Dictionary<Type, LocalBuilder> DeclareValueTypeLocals(this ILGenerator ilGenerator, IEnumerable<Type> types)
        {
            return types
                .Distinct()
                .Where(x => x.IsValueType && (x.IsNullableType() || !LdcDictionary.ContainsKey(x)))
                .ToDictionary(x => x, ilGenerator.DeclareLocal);
        }

        public static void EmitStackColumnValue(this ILGenerator ilGenerator, int index)
        {
            ilGenerator.Emit(OpCodes.Ldarg_1);                 // [IDataRecord]
            ilGenerator.EmitLdcI4(index);                       // [IDataRecord][index]
            ilGenerator.Emit(OpCodes.Callvirt, GetValue);     // [Value]
        }

        public static void EmitCheckDbNull(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Dup);
            ilGenerator.Emit(OpCodes.Isinst, typeof(DBNull));
        }

        public static void EmitStackDefault(this ILGenerator ilGenerator, Type type, Dictionary<Type, LocalBuilder> valueTypeLocals)
        {
            if (type.IsValueType)
            {
                if (LdcDictionary.TryGetValue(type.IsEnum ? type.GetEnumUnderlyingType() : type, out var action))
                {
                    action(ilGenerator);
                }
                else
                {
                    var local = valueTypeLocals[type];

                    ilGenerator.Emit(local.LocalIndex < 256 ? OpCodes.Ldloca_S : OpCodes.Ldloca, local);
                    ilGenerator.Emit(OpCodes.Initobj, type);
                    ilGenerator.Emit(local.LocalIndex < 256 ? OpCodes.Ldloc_S : OpCodes.Ldloc, local);
                }
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldnull);
            }
        }

        public static void EmitConvertByField(this ILGenerator ilGenerator, FieldInfo field, LocalBuilder local)
        {
            ilGenerator.Emit(local.LocalIndex < 256 ? OpCodes.Stloc_S : OpCodes.Stloc, local);      // [Value] :

            ilGenerator.Emit(OpCodes.Ldarg_0);                                                      // [Value] : [Holder]
            ilGenerator.Emit(OpCodes.Ldfld, field);                                                 // [Value] : [Converter]

            ilGenerator.Emit(local.LocalIndex < 256 ? OpCodes.Ldloc_S : OpCodes.Ldloc, local);      // [Converter][Value]

            var method = typeof(Func<object, object>).GetMethod("Invoke");
            ilGenerator.Emit(OpCodes.Callvirt, method);                                             // [Value(Converted)]
        }

        public static void EmitTypeConversion(this ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsNullableType())
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    var nullableCtor = type.GetConstructor(new[] { underlyingType });

                    ilGenerator.Emit(OpCodes.Unbox_Any, underlyingType);
                    ilGenerator.Emit(OpCodes.Newobj, nullableCtor);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Unbox_Any, type);
                }
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        public static void EmitChangeToNullable(this ILGenerator ilGenerator, Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            var nullableCtor = type.GetConstructor(new[] { underlyingType });

            ilGenerator.Emit(OpCodes.Newobj, nullableCtor);
        }

        public static void EmitSetter(this ILGenerator ilGenerator, PropertyInfo pi)
        {
            ilGenerator.Emit(pi.DeclaringType.IsValueType ? OpCodes.Call : OpCodes.Callvirt, pi.SetMethod);
        }
    }
}
