using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ParameterTypeCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText(@"
public class Target
{
    private static void Test(int p1, string p2, Parameter p3)
    {
        Setup(p1);
        if (!System.String.IsNullOrEmpty(p2))
        {
            Setup(p2);
        }
        Setup(p3.Id);
        Setup(p3.Values[1]);
    }

    private static T Setup<T>(T value) => value;
}

public class Parameter
{
    public int Id { get; set; }

    public int[] Values { get; set; }
}
");

            var compilation = CSharpCompilation.Create("Work", syntaxTrees: new[] { tree })
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Tuple).GetTypeInfo().Assembly.Location));
            var model = compilation.GetSemanticModel(tree);

            foreach (var method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var methodSymbol = model.GetSymbolInfo(invocation).Symbol;
                    if (methodSymbol.ContainingType.Name == "Setup")
                    {
                        continue;
                    }

                    var arguments = invocation.ArgumentList;
                    var arg = arguments.Arguments[0].Expression;

                    Debug.WriteLine("--" + arg.GetType());
                    var type = model.GetTypeInfo(arg);
                    Debug.WriteLine(arg.ToFullString());
                    Debug.WriteLine(type.Type?.Name);
                    //Debug.WriteLine(symbol.GetType());
                }
            }
        }
    }
}
