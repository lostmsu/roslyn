namespace FormattingTest
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Formatting;

    class Program
    {
        static void Main(string[] args)
        {
            string sourceCode = File.ReadAllText(args[0]);
            var ast = CSharpSyntaxTree.ParseText(sourceCode);
            var unformatted = new WhitespaceRemover().Visit(
                ast.GetRoot()
                )
                ;
            var workspace = new AdhocWorkspace();
            workspace.Options = workspace.Options
                .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n");
            Console.Write($"formatting {args[0]}...");
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                var reformatted = Formatter.Format(unformatted, workspace, workspace.Options);
                string newCode = reformatted.GetText().ToString();
                Console.WriteLine(newCode.Last());
            }
            Console.WriteLine("OK");
            Console.WriteLine($"time: {stopwatch.ElapsedMilliseconds / 1000}s");
        }
    }

    public class WhitespaceRemover : CSharpSyntaxRewriter
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
        public override SyntaxNode Visit(SyntaxNode node) => node == null ? null : base.Visit(node).WithoutTrivia();
    }
}
