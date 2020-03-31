namespace SelectorTest
{
    using System.Reflection;

    public class ConstructorMapInfo
    {
        public ConstructorInfo Info { get; }

        public int[] Indexes { get; }

        public ConstructorMapInfo(ConstructorInfo ci, int[] indexes)
        {
            Info = ci;
            Indexes = indexes;
        }
    }

    public class PropertyMapInfo
    {
        public PropertyInfo Info { get; }

        public int Index { get; }

        public PropertyMapInfo(PropertyInfo pi, int index)
        {
            Info = pi;
            Index = index;
        }
    }

    public class TypeMapInfo
    {
        public ConstructorMapInfo Constructor { get; }

        public PropertyMapInfo[] Properties { get; }

        public TypeMapInfo(ConstructorMapInfo constructor, PropertyMapInfo[] properties)
        {
            Constructor = constructor;
            Properties = properties;
        }
    }
}
