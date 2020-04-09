using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Smart;
using Smart.Mock.Data;
using Smart.Reflection.Emit;

namespace ConstructorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ci = typeof(Data).GetConstructors()[0];
            var entries = ci.GetParameters().Select((x, i) => new Factory.MapEntry(i, null)).ToArray();

            var factory = new Factory();
            var mapper = factory.CreateMapper<Data>(ci, entries);

            var reader = new MockDataReader(
                new[]
                {
                    new MockColumn(typeof(int), "Id"),
                },
                new []
                {
                    new object[] { 1},
                    new object[] { DBNull.Value},
                });

            var entity = mapper(reader);
        }
    }

    public class Data
    {
    }

    public class Factory
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
                        new AssemblyName("NewResultMapperFactoryAssembly"),
                        AssemblyBuilderAccess.Run);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(
                        "NewResultMapperFactoryModule");
                }

                return moduleBuilder;
            }
        }

        public Func<IDataRecord, T> CreateMapper<T>(ConstructorInfo ci, MapEntry[] entries)
        {
            var type = typeof(T);
            var holder = CreateHolder(ci, entries);
            var holderType = holder.GetType();

            var getValue = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue));

            var dynamicMethod = new DynamicMethod(string.Empty, type, new[] { holderType, typeof(IDataRecord) }, true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // local variables
            var objectLocal = entries.Any(x => x.Converter != null) ? ilGenerator.DeclareLocal(typeof(object)) : null;
            var valueTypeLocal = entries
                .Select(x => ci.GetParameters()[x.Index].ParameterType)
                .Where(x => x.IsValueType && (x.IsNullableType() || !LdcDictionary.ContainsKey(x)))
                .Distinct()
                .ToDictionary(x => x, x => ilGenerator.DeclareLocal(x));
            var isShort = valueTypeLocal.Count + (objectLocal != null ? 1 : 0) <= 256;

            foreach (var entry in entries)
            {
                var parameterType = ci.GetParameters()[entry.Index].ParameterType;

                var hasValueLabel = ilGenerator.DefineLabel();
                var next = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Ldarg_1); // [IDataRecord]
                ilGenerator.EmitLdcI4(entry.Index); // [IDataRecord][index]

                ilGenerator.Emit(OpCodes.Callvirt, getValue);   // [Value]

                // Check DBNull
                ilGenerator.Emit(OpCodes.Dup);  // [Value][Value]
                ilGenerator.Emit(OpCodes.Isinst, typeof(DBNull));   // [Value]
                ilGenerator.Emit(OpCodes.Brfalse_S, hasValueLabel);

                // ----------------------------------------
                // Null
                // ----------------------------------------

                // [Value]

                ilGenerator.Emit(OpCodes.Pop);
                if (parameterType.IsValueType)
                {
                    if (LdcDictionary.TryGetValue(parameterType.IsEnum ? parameterType.GetEnumUnderlyingType() : parameterType, out var action))
                    {
                        action(ilGenerator);
                    }
                    else
                    {
                        var local = valueTypeLocal[parameterType];

                        ilGenerator.Emit(isShort ? OpCodes.Ldloca_S : OpCodes.Ldloca, local);
                        ilGenerator.Emit(OpCodes.Initobj, parameterType);
                        ilGenerator.Emit(isShort ? OpCodes.Ldloc_S : OpCodes.Ldloc, local);
                    }
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
                ilGenerator.Emit(OpCodes.Br_S, next);

                // ----------------------------------------
                // Value
                // ----------------------------------------

                // [Value]

                ilGenerator.MarkLabel(hasValueLabel);

                if (entry.Converter != null)
                {
                    ilGenerator.Emit(isShort ? OpCodes.Stloc_S : OpCodes.Stloc, objectLocal);  // [Value] :

                    var field = holderType.GetField($"parser{entry.Index}");
                    ilGenerator.Emit(OpCodes.Ldarg_0);                                          // [Value] : [Holder]
                    ilGenerator.Emit(OpCodes.Ldfld, field);                                     // [Value] : [Converter]

                    ilGenerator.Emit(isShort ? OpCodes.Ldloc_S : OpCodes.Ldloc, objectLocal);  // [Converter][Value]

                    var method = typeof(Func<object, object>).GetMethod("Invoke");
                    ilGenerator.Emit(OpCodes.Callvirt, method); // [Value(Converted)]
                }

                // [MEMO] 最適化Converterがある場合は以下の共通ではなくなる
                if (parameterType.IsValueType)
                {
                    if (parameterType.IsNullableType())
                    {
                        var underlyingType = Nullable.GetUnderlyingType(parameterType);
                        var nullableCtor = parameterType.GetConstructor(new[] { underlyingType });

                        ilGenerator.Emit(OpCodes.Unbox_Any, underlyingType);
                        ilGenerator.Emit(OpCodes.Newobj, nullableCtor);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Unbox_Any, parameterType);
                    }
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Castclass, parameterType);
                }

                // ----------------------------------------
                // Next
                // ----------------------------------------

                ilGenerator.MarkLabel(next);
            }

            // New
            ilGenerator.Emit(OpCodes.Newobj, ci);

            // TODO Property

            ilGenerator.Emit(OpCodes.Ret);

            var funcType = typeof(Func<,>).MakeGenericType(typeof(IDataRecord), type);
            return (Func<IDataRecord, T>)dynamicMethod.CreateDelegate(funcType, holder);
        }

        private object CreateHolder(ConstructorInfo ci, MapEntry[] entries)
        {
            var typeBuilder = ModuleBuilder.DefineType(
                $"Holder_{typeNo}",
                TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed |
                TypeAttributes.BeforeFieldInit);
            typeNo++;

            // Define setter fields
            foreach (var entry in entries)
            {
                if (entry.Converter != null)
                {
                    typeBuilder.DefineField(
                        $"parser{entry.Index}",
                        typeof(Func<object, object>),
                        FieldAttributes.Public);
                }
            }

            var typeInfo = typeBuilder.CreateTypeInfo();
            var holderType = typeInfo.AsType();
            var holder = Activator.CreateInstance(holderType);

            foreach (var entry in entries)
            {
                if (entry.Converter != null)
                {
                    var field = holderType.GetField($"parser{entry.Index}");
                    field.SetValue(holder, entry.Converter);
                }
            }

            return holder;

        }

        public class MapEntry
        {
            public readonly int Index;

            public readonly Func<object, object> Converter;

            public MapEntry(int index, Func<object, object> converter)
            {
                Index = index;
                Converter = converter;
            }
        }
    }
}
