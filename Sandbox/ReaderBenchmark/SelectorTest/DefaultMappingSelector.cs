namespace SelectorTest
{
    using System;
    using System.Linq;

    public class DefaultMappingSelector : IMappingSelector
    {
        public TypeMapInfo Select(Type type, ColumnInfo[] columns)
        {
            var columnMap = columns.ToDictionary(x => x.Name);

            //var ctos = type.GetConstructors()
            //    .OrderByDescending(x => x.GetParameters().Length)
            //    .Where()

            // TODO FindConstructor

            throw new NotImplementedException();
        }
    }
}
