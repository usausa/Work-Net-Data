using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WorkTreeWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText(Source);
            var compilation = CSharpCompilation.Create("Work", syntaxTrees: new[] { tree })
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Tuple).GetTypeInfo().Assembly.Location));
            var model = compilation.GetSemanticModel(tree);

            Debug.IndentSize = 2;
            foreach (var syntax in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var first = syntax.ChildNodes().First();
                if ((first is IdentifierNameSyntax identifier) && (identifier.Identifier.Text == "Setup"))
                {
                    Debug.WriteLine("");
                    Debug.WriteLine($"----{syntax.ToFullString().Trim()}");
                    Tree(syntax);
                }
            }
        }

        private static void Tree(SyntaxNode node)
        {
            Debug.WriteLine($@"{node.GetType().Name} {node.ToFullString().Trim()}");

            Debug.IndentLevel++;
            foreach (var child in node.ChildNodes())
            {
                Tree(child);
            }
            Debug.IndentLevel--;
        }

        private const string Source =
            @"
        public class Target
        {
            public void Test(
                Index index,
                Parameter p,
                int id,
                int[] values,
                ChildParameter child,
                ChildParameter[] children,
                System.Collections.Generic.Dictionary<int, string> map,
                System.Collections.Generic.Dictionary<int, ChildParameter> childMap,
                System.Collections.Generic.Dictionary<int, int[]> nested)
            {
                Setup(id);
                Setup(values);
                Setup(values[0]);
                Setup(values[index.Get()]);
                Setup(child.Id);
                Setup(child?.Id);
                Setup(child?.Id ?? 0);
                Setup(children[0].Id);
                Setup(children?[0].Id);
                Setup(children?[0]?.Id);
                Setup(children?[0]?.Id ?? 0);
                Setup(children[index.Get()].Id);
                Setup(map[0]);
                Setup(childMap[0].Id);
                Setup(childMap[index.Get()].Id);
                Setup(childMap[0].Id);
                Setup(nested[0][0]);

                Setup(p.Id);
                Setup(p.Values);
                Setup(p.Values[0]);
                Setup(p.Values[index.Get()]);
                Setup(p.Child.Id);
                Setup(p.Child?.Id);
                Setup(p.Child?.Id ?? 0);
                Setup(p.Children[0].Id);
                Setup(p.Children?[0].Id);
                Setup(p.Children?[0]?.Id);
                Setup(p.Children?[0]?.Id ?? 0);
                Setup(p.Children[index.Get()].Id);
                Setup(p.Map[0]);
                Setup(p.ChildMap[0].Id);
                Setup(p.ChildMap[index.Get()].Id);
                Setup(p.ChildMap[0].Id);
                Setup(p.Nested[0][0]);

                var temp = id;
                Setup(temp);
            }

            private static T Setup<T>(T value) => value;
        }

        public class Index
        {
            private readonly int index;

            public Index(int index)
            {
                this.index = index;
            }

            public int Get() => index;
        }

        public class ChildParameter
        {
            public int Id { get; set; }
        }

        public class Parameter
        {
            public int Id { get; set; }

            public int[] Values { get; set; }

            public ChildParameter Child { get; set; }

            public ChildParameter[] Children { get; set; }

            public System.Collections.Generic.Dictionary<int, string> Map { get; set; }

            public System.Collections.Generic.Dictionary<int, ChildParameter> ChildMap { get; set; }

            public System.Collections.Generic.Dictionary<int, int[]> Nested { get; set; }
        }
    }
";
    }
}
