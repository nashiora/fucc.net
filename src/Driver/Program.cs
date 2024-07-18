using Fucc.FrontEnd.C;
using Fucc.FrontEnd.Laye;

namespace Fucc.Driver;

public static class Program
{
    public static int Main(string[] args)
    {
        //CoreTest.Test();
        //CTest.Test();

        var sourceText = SourceText.CreateFromFile("./test/tokens.laye");
        var layeLexer = new LayeLexer(new(sourceText));

        while (!layeLexer.State.IsAtEnd)
        {
            var token = layeLexer.ReadToken();
            Console.WriteLine($"{token.SyntaxKind} :: {token.SourceLocation.SourceSpan}");
        }

        return 0;
    }
}
