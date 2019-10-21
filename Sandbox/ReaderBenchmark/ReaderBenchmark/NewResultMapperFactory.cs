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

        public bool IsMatch(Type type) => true;

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

            ilGenerator.Emit(OpCodes.Newobj, ci);

            foreach (var entry in entries)
            {
                // TODO
                // GetValue
                // isnull
                //   class
                //   struct init
                // value
                //   class
                //   basic
                //   nullable
                //   other? exp?

                var hasValueLabel = ilGenerator.DefineLabel();
                var setPropertyLabel = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Dup);  // [T][T]

                ilGenerator.Emit(OpCodes.Ldarg_1); // [T][T][IDataRecord]
                ilGenerator.EmitLdcI4(entry.Index); // [T][T][IDataRecord][index]

                ilGenerator.Emit(OpCodes.Callvirt, getValue);   // [T][T][Value]

                // Check DBNull
                ilGenerator.Emit(OpCodes.Dup);  // [T][T][Value][Value]
                ilGenerator.Emit(OpCodes.Isinst, typeof(DBNull));   // [T][T][Value]
                ilGenerator.Emit(OpCodes.Brfalse_S, hasValueLabel);

                // Null
                ilGenerator.Emit(OpCodes.Pop);
                if (entry.Property.PropertyType.IsValueType)
                {
                    // TODO Nullable, Struct, Ex
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }

                ilGenerator.Emit(OpCodes.Br_S, setPropertyLabel);

                // Value
                ilGenerator.MarkLabel(hasValueLabel);

                if (entry.Property.PropertyType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Unbox_Any, entry.Property.PropertyType);
                    // TODO Nullable, Struct, Ex
                }

                // Set
                ilGenerator.MarkLabel(setPropertyLabel);

                ilGenerator.Emit(OpCodes.Callvirt, entry.Property.SetMethod);

                //if (entry.Converter == null)
                //{
                //    var method = getValueMethod.MakeGenericMethod(entry.Property.PropertyType);
                //    ilGenerator.Emit(OpCodes.Call, method);
                //}
                //else
                //{
                //    var field = holderType.GetField($"parser{entry.Index}");
                //    ilGenerator.Emit(OpCodes.Ldarg_0);
                //    ilGenerator.Emit(OpCodes.Ldfld, field);
                //    var method = getValueWithConvertMethod.MakeGenericMethod(entry.Property.PropertyType);
                //    ilGenerator.Emit(OpCodes.Call, method);
                //}
            }

            // TODO ValueType ?

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

                // TODO Converter
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static T GetValue<T>(IDataRecord reader, int index)
        //{
        //    var value = reader.GetValue(index);
        //    return value is DBNull ? default : (T)value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static T GetValueWithConvert<T>(IDataRecord reader, int index, Func<object, object> parser)
        //{
        //    var value = reader.GetValue(index);
        //    return value is DBNull ? default : (T)parser(value);
        //}

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
