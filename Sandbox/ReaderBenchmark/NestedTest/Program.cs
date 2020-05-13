using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NestedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Dump(Query1().MapOneToMany(p => p.Id, (p, cs) => p.Children = cs));
            Dump(Query2().MapOneToMany(p => p.Id, (p, cs) => p.Children = cs));
        }

        static void Dump(IEnumerable<ParentEntity> source)
        {
            Debug.WriteLine("----------");
            foreach (var parent in source)
            {
                Debug.WriteLine($"Id=[{parent.Id}], Name=[{parent.Name}], Children=[{parent.Children.Count}]");
                foreach (var child in parent.Children)
                {
                    Debug.WriteLine($"  Id=[{child.Id}], Name=[{child.Name}]");
                }
            }
        }

        static IEnumerable<Tuple<ParentEntity, ChildEntity>> Query1()
        {
            yield return Tuple.Create(new ParentEntity { Id = 1, Name = "P1" }, new ChildEntity { Id = 11, Name = "C11" });
            yield return Tuple.Create(new ParentEntity { Id = 1, Name = "P1" }, new ChildEntity { Id = 12, Name = "C12" });
            yield return Tuple.Create(new ParentEntity { Id = 2, Name = "P2" }, new ChildEntity { Id = 21, Name = "C21" });
        }

        static IEnumerable<Tuple<ParentEntity, ChildEntity>> Query2()
        {
            yield return Tuple.Create(new ParentEntity { Id = 1, Name = "P1" }, new ChildEntity { Id = 11, Name = "C11" });
            yield return Tuple.Create(new ParentEntity { Id = 2, Name = "P2" }, new ChildEntity { Id = 21, Name = "C21" });
            yield return Tuple.Create(new ParentEntity { Id = 2, Name = "P2" }, new ChildEntity { Id = 22, Name = "C22" });
        }
    }

    public class ParentEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ChildEntity> Children { get; set; }
    }

    public class ChildEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class NestedExtensions
    {
        public static IEnumerable<TP> MapOneToMany<TP, TC, TPKey>(
            this IEnumerable<Tuple<TP, TC>> source,
            Func<TP, TPKey> parentKeySelector,
            Action<TP, List<TC>> combiner)
        {
            return MapOneToMany(
                source,
                x => x.Item1,
                x => x.Item2,
                parentKeySelector,
                EqualityComparer<TPKey>.Default,
                combiner);
        }

        public static IEnumerable<TP> MapOneToMany<T, TP, TC, TPKey>(
            this IEnumerable<T> source,
            Func<T, TP> parentSelector,
            Func<T, TC> childSelector,
            Func<TP, TPKey> parentKeySelector,
            IEqualityComparer<TPKey> keyComparer,
            Action<TP, List<TC>> combiner)
        {
            using (var en = source.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    var item = en.Current;
                    var parent = parentSelector(item);
                    var key = parentKeySelector(parent);
                    var children = new List<TC>
                    {
                        childSelector(item)
                    };

                    while (en.MoveNext())
                    {
                        var newItem = en.Current;
                        var newParent = parentSelector(newItem);
                        var newKey = parentKeySelector(newParent);

                        if (!keyComparer.Equals(key, newKey))
                        {
                            combiner(parent, children);
                            yield return parent;

                            key = newKey;
                            children = new List<TC>();
                        }

                        children.Add(childSelector(newItem));
                    }

                    combiner(parent, children);
                    yield return parent;
                }
            }
        }
    }
}
