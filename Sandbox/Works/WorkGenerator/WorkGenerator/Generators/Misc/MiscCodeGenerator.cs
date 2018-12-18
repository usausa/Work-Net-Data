namespace WorkGenerator.Generators.Misc
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MiscCodeGenerator
    {
        private readonly string source;

        public MiscCodeGenerator(string source)
        {
            this.source = source;
        }

        public void Test()
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetRoot();

            // Usings
            var usings = new HashSet<string>(new[] { "System" });
            foreach (var item in root
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(ud => ud.Name.ToString()))
            {
                usings.Add(item);
            }

            // Classes
            var classes = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .SelectMany(nds => nds.DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Select(ids => CreateClassMetadata(nds, ids))
                    .Where(x => x != null))
                .ToArray();

            // Dump
            foreach (var ns in usings.OrderBy(x => x))
            {
                Debug.WriteLine(ns);
            }

            // TODO
        }

        private static bool IsTargetAttribute(AttributeSyntax syntax)
        {
            var name = syntax.Name.ToString().Split('.').Last();
            return ((name == "Dao") || (name == "DaoAttribute")) &&
                   ((syntax.ArgumentList?.Arguments.Count ?? 0) == 0);
        }

        private static ClassMetadata CreateClassMetadata(NamespaceDeclarationSyntax nds, InterfaceDeclarationSyntax ids)
        {
            var atrs = ids.AttributeLists
                .SelectMany(als => als.Attributes)
                .FirstOrDefault(IsTargetAttribute);
            if (atrs == null)
            {
                return null;
            }

            // TODO
            return new ClassMetadata
            {
                Namespace = nds.Name.ToString(),
                //Attribute = atrs.Name.ToString(),
                Name = ids.Identifier.Text,
                Methods = ids.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Select(CreateMethodMetadata)
                    .ToArray()
            };
        }

        private static MethodMetadata CreateMethodMetadata(MethodDeclarationSyntax mds)
        {
            // TODO Attr
            return new MethodMetadata
            {
                Name = mds.Identifier.Text,
                ReturnType = mds.ReturnType.ToString()
                // TODO mds.ParameterList
            };
        }
    }
}
