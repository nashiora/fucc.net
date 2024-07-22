// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Fucc.FrontEnd.C;
using Fucc.FrontEnd.Laye;

namespace Fucc.Driver;

public static class Program
{
    private static void PrintSourceTokens<TLexer, TToken>(TLexer lexer)
        where TLexer : ILexer<TToken>
        where TToken : ILexerToken
    {
        TToken token;
        do
        {
            token = lexer.ReadToken();
            Console.WriteLine(token);
        }
        while (!token.IsEndOfFileToken);
    }

    private static void PrintLayeSourceTokens(string sourceFilePath)
    {
        Console.WriteLine($"Lexing Laye source from '{sourceFilePath}'");
        var sourceText = SourceText.CreateFromFile(sourceFilePath);
        var layeLexer = new LayeLexer(new(sourceText));
        PrintSourceTokens<LayeLexer, LayeToken>(layeLexer);
        Console.WriteLine();
    }

    private static void PrintCSourceTokens(string sourceFilePath)
    {
        Console.WriteLine($"Lexing C source from '{sourceFilePath}'");
        var sourceText = SourceText.CreateFromFile(sourceFilePath);
        var cLexer = new CLexer(new(sourceText));
        PrintSourceTokens<CLexer, CToken>(cLexer);
        Console.WriteLine();
    }

    public static int Main(string[] args)
    {
        PrintLayeSourceTokens("./test/tokens.laye");
        PrintCSourceTokens("./test/tokens.c");
        return 0;
    }
}
