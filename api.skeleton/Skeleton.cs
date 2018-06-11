namespace api.skeleton
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.MSBuild;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class Information
    {
        private const string HTMLStartTags = "<tr><td>";
        private const string HTMLEndTags = "</td><td></td></tr>";
        public string WriteProjectContent(string pathToSolution, string projectName)
        {
            StringBuilder sb = new StringBuilder();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solutionToAnalyze = workspace.OpenSolutionAsync(pathToSolution).Result;

            Project projectToAnalyze = solutionToAnalyze.Projects
                                    .Where((p) => p.Name == projectName)
                                    .FirstOrDefault();

            var documents = projectToAnalyze.Documents
                         .Where(s => s.Name.EndsWith(".cs") && !s.FilePath.Contains("App_Start"))
                         .ToList();

            Task<SyntaxTree> tree = null;

            foreach (var d in documents)
            {
                tree = d.GetSyntaxTreeAsync();
                var model = d.GetSemanticModelAsync();

                IEnumerable<ClassDeclarationSyntax> classes = tree.Result
                    .GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>();


                foreach (var c in classes)
                {
                    var symbol = (INamedTypeSymbol)model.Result.GetDeclaredSymbol(c);

                    IEnumerable<PropertyDeclarationSyntax> properties = c.SyntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>();

                    sb.AppendLine(WritePropertyInfo(properties));

                    sb.AppendLine(WriteGenericMethodInfo(symbol));
                }

            }

            return sb.ToString();

        }

        private string WritePropertyInfo(IEnumerable<PropertyDeclarationSyntax> dependency)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<p>Dependency properties</p>");
            sb.AppendLine("<table>");
            foreach (var d in dependency)
            {
                sb.AppendLine($"<td>{(d as PropertyDeclarationSyntax).Type.ToFullString()}</td><td></td></tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }
        private string WriteGenericMethodInfo(INamedTypeSymbol symbol)
        {
            StringBuilder sb = new StringBuilder();
            if (symbol.Kind == SymbolKind.NamedType && symbol.BaseType.IsGenericType)
            {

                string businessName = string.Empty;
                string dtoName = string.Empty;
                List<ITypeSymbol> arguments = new List<ITypeSymbol>();
                //TODO: Fix specific type and count argument limit
                //      Create more test projects
                businessName = symbol.BaseType.TypeArguments[0].Name;
                dtoName = symbol.BaseType.TypeArguments[1].Name;
                arguments = symbol.BaseType.TypeArguments.Where(t => t.GetMembers().Count() != 0).ToList();

                if (!string.IsNullOrEmpty(businessName))
                    sb.AppendLine($"\"{symbol.Name.ToString()}\" API with \"{businessName}\" interface");


                sb.AppendLine(WriteArgumentInfo(arguments, SymbolKind.Method, true));
                sb.AppendLine($"\r\n\"{dtoName}\" used as data structure"
                    + "");
                sb.AppendLine(WriteArgumentInfo(arguments, SymbolKind.Property, false));
                sb.AppendLine("-----------------------------------------------------------------------------------------<br />");

                arguments.Clear();

            }
            return sb.ToString();
        }
        private string WriteArgumentInfo(List<ITypeSymbol> arguments, SymbolKind kind, bool isAbstract)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            foreach (var entity in arguments.Where(be => be.IsAbstract == isAbstract).ToList())
            {
                var businessOperations = entity.GetMembers().Where(s => s.Kind == kind).Select(s => HTMLStartTags + new String(s.Name.ToString().ToCharArray()) + ": " + HTMLEndTags);
                sb.AppendLine(string.Join("\r\n", businessOperations));

            }
            sb.AppendLine("</table>");

            return sb.ToString();

        }


    }


}
