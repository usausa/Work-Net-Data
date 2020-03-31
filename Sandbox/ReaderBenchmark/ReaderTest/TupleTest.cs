using System;

namespace ReaderTest
{
    public class Entity1
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Entity2
    {
        public string Id { get; set; }

        public int Value { get; set; }
    }

    public static class TupleTest
    {
        public static ValueTuple<Entity1, Entity2> CreateValueTuple()
        {
            var o1 = new Entity1();
            o1.Id = default;
            o1.Name = null;

            var o2 = new Entity2();
            o2.Id = null;
            o2.Value = default;

            return new ValueTuple<Entity1, Entity2>(o1, o2);
        }
    }
}
