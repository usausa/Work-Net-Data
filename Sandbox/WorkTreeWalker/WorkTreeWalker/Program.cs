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
        static void Main()
        {
            var tree = CSharpSyntaxTree.ParseText(Source);
            var compilation = CSharpCompilation.Create("Work", syntaxTrees: new[] { tree })
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Tuple).GetTypeInfo().Assembly.Location));
            var model = compilation.GetSemanticModel(tree);

            Debug.IndentSize = 2;
            foreach (var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var first = invocation.ChildNodes().First();
                    if ((first is IdentifierNameSyntax identifier) && (identifier.Identifier.Text == "Setup"))
                    {
                        Debug.WriteLine("");
                        Debug.WriteLine($"----{invocation.ToFullString().Trim()}");
                        Tree(invocation.ArgumentList.Arguments[0].Expression, model);

                        var type = model.GetTypeInfo(invocation.ArgumentList.Arguments[0].Expression);
                        Debug.WriteLine($"Type = {type.Type}");

                        // TODO get attribute : parameter, property, property has element, method
                    }
                }
            }
        }

        private static void Tree(SyntaxNode syntax, SemanticModel model)
        {
            Debug.IndentLevel++;
            Debug.WriteLine($"{syntax.GetType()} : {syntax.ToFullString().Trim()}");
            foreach (var child in syntax.ChildNodes())
            {
                Tree(child, model);
            }
            Debug.IndentLevel--;
        }

        private const string Source =
            @"
        public class TestAttribute : System.Attribute
        {
            public string Value { get; }

            public TestAttribute(string value)
            {
                Value = value;
            }
        }

        public class Target
        {
            public void Test(
                Index index,
                [Test(""p"")]
                Parameter p,
                [Test(""id"")]
                int id,
                [Test(""values"")]
                int[] values,
                [Test(""child"")]
                ChildParameter child,
                [Test(""children"")]
                ChildParameter[] children,
                [Test(""map"")]
                System.Collections.Generic.Dictionary<int, string> map,
                [Test(""childMap"")]
                System.Collections.Generic.Dictionary<int, ChildParameter> childMap,
                [Test(""nested"")]
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
                Setup(p.Call());

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
            [Test(""Id"")]
            public int Id { get; set; }
        }

        public class Parameter
        {
            [Test(""Id"")]
            public int Id { get; set; }

            [Test(""Values"")]
            public int[] Values { get; set; }

            [Test(""Child"")]
            public ChildParameter Child { get; set; }

            [Test(""Children"")]
            public ChildParameter[] Children { get; set; }

            [Test(""Map"")]
            public System.Collections.Generic.Dictionary<int, string> Map { get; set; }

            [Test(""ChildMap"")]
            public System.Collections.Generic.Dictionary<int, ChildParameter> ChildMap { get; set; }

            [Test(""Nested"")]
            public System.Collections.Generic.Dictionary<int, int[]> Nested { get; set; }

            [Test(""Call"")]
            public int Call() => 0;
        }
    }
";
    }
}
