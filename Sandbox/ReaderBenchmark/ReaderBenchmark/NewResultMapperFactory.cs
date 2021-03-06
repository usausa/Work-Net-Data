﻿using System.Linq;
using Smart;
using Smart.Reflection.Emit;

namespace ReaderBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class NewResultMapperFactory
    {
        public static NewResultMapperFactory Instance { get; } = new NewResultMapperFactory();

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

        //public bool IsMatch(Type type) => true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public Func<IDataRecord, T> CreateMapper<T>(Type type, ColumnInfo[] columns)
        {
            var entries = CreateMapEntries(type, columns);
            var holder = CreateHolder(entries);
            var holderType = holder.GetType();

            var ci = type.GetConstructor(Type.EmptyTypes);
            if (ci is null)
            {
                throw new ArgumentException($"Default constructor not found. type=[{type.FullName}]", nameof(type));
            }

            var getValue = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue));

            var dynamicMethod = new DynamicMethod(string.Empty, type, new[] { holderType, typeof(IDataRecord) }, true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // local variables
            var objectLocal = entries.Any(x => x.Converter != null) ? ilGenerator.DeclareLocal(typeof(object)) : null;
            var valueTypeLocal = entries
                .Select(x => x.Property.PropertyType)
                .Where(x => x.IsValueType && (x.IsNullableType() || !LdcDictionary.ContainsKey(x)))
                .Distinct()
                .ToDictionary(x => x, x => ilGenerator.DeclareLocal(x));
            var isShort = valueTypeLocal.Count + (objectLocal != null ? 1 : 0) <= 256;

            // New
            ilGenerator.Emit(OpCodes.Newobj, ci);

            foreach (var entry in entries)
            {
                var propertyType = entry.Property.PropertyType;

                var hasValueLabel = ilGenerator.DefineLabel();
                var next = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Dup);  // [T][T]

                ilGenerator.Emit(OpCodes.Ldarg_1); // [T][T][IDataRecord]
                ilGenerator.EmitLdcI4(entry.Index); // [T][T][IDataRecord][index]

                ilGenerator.Emit(OpCodes.Callvirt, getValue);   // [T][T][Value]

                // Check DBNull
                ilGenerator.Emit(OpCodes.Dup);  // [T][T][Value][Value]
                ilGenerator.Emit(OpCodes.Isinst, typeof(DBNull));   // [T][T][Value]
                ilGenerator.Emit(OpCodes.Brfalse_S, hasValueLabel);

                // ----------------------------------------
                // Null
                // ----------------------------------------

                // [T][T][Value]

                ilGenerator.Emit(OpCodes.Pop);  // [T][T]
                if (propertyType.IsValueType)
                {
                    if (LdcDictionary.TryGetValue(propertyType.IsEnum ? propertyType.GetEnumUnderlyingType() : propertyType, out var action))
                    {
                        action(ilGenerator);
                    }
                    else
                    {
                        var local = valueTypeLocal[entry.Property.PropertyType];

                        ilGenerator.Emit(isShort ? OpCodes.Ldloca_S : OpCodes.Ldloca, local);
                        ilGenerator.Emit(OpCodes.Initobj, entry.Property.PropertyType);
                        ilGenerator.Emit(isShort ? OpCodes.Ldloc_S : OpCodes.Ldloc, local);
                    }
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }

                ilGenerator.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, entry.Property.SetMethod);

                ilGenerator.Emit(OpCodes.Br_S, next);

                // ----------------------------------------
                // Value
                // ----------------------------------------

                // [T][T][Value]

                ilGenerator.MarkLabel(hasValueLabel);

                if (entry.Converter != null)
                {
                    ilGenerator.Emit(isShort ? OpCodes.Stloc_S : OpCodes.Stloc, objectLocal);  // [Value] : [T][T]

                    var field = holderType.GetField($"parser{entry.Index}");
                    ilGenerator.Emit(OpCodes.Ldarg_0);                                          // [Value] : [T][T][Holder]
                    ilGenerator.Emit(OpCodes.Ldfld, field);                                     // [Value] : [T][T][Converter]

                    ilGenerator.Emit(isShort ? OpCodes.Ldloc_S : OpCodes.Ldloc, objectLocal);  // [T][T][Converter][Value]

                    var method = typeof(Func<object, object>).GetMethod("Invoke");
                    ilGenerator.Emit(OpCodes.Callvirt, method); // [T][T][Value(Converted)]
                }

                // [MEMO] 最適化Converterがある場合は以下の共通ではなくなる
                if (entry.Property.PropertyType.IsValueType)
                {
                    if (entry.Property.PropertyType.IsNullableType())
                    {
                        var underlyingType = Nullable.GetUnderlyingType(entry.Property.PropertyType);
                        var nullableCtor = entry.Property.PropertyType.GetConstructor(new[] { underlyingType });

                        ilGenerator.Emit(OpCodes.Unbox_Any, underlyingType);
                        ilGenerator.Emit(OpCodes.Newobj, nullableCtor);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Unbox_Any, entry.Property.PropertyType);
                    }
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Castclass, entry.Property.PropertyType);
                }

                ilGenerator.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, entry.Property.SetMethod);

                // ----------------------------------------
                // Next
                // ----------------------------------------

                ilGenerator.MarkLabel(next);
            }

            ilGenerator.Emit(OpCodes.Ret);

            var funcType = typeof(Func<,>).MakeGenericType(typeof(IDataRecord), type);
            return (Func<IDataRecord, T>)dynamicMethod.CreateDelegate(funcType, holder);
        }

        private static MapEntry[] CreateMapEntries(Type type, ColumnInfo[] columns)
        {
            var list = new List<MapEntry>();
            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var pi = type.GetProperty(column.Name);

                list.Add(new MapEntry(i, pi, null));
            }

            return list.ToArray();
        }

        private object CreateHolder(MapEntry[] entries)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]
        private sealed class MapEntry
        {
            public readonly int Index;

            public readonly PropertyInfo Property;

            public readonly Func<object, object> Converter;

            public MapEntry(int index, PropertyInfo property, Func<object, object> converter)
            {
                Index = index;
                Property = property;
                Converter = converter;
            }
        }
    }
}
