// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Fucc.FrontEnd.C;

public class CPreprocessorState
{
    public void InitializeFrom(CPreprocessorState other)
    {
    }
}

public class CPreprocessorMacroDefinition<TToken>
    where TToken : ICPreprocessorToken
{
    public required string Name { get; init; }
    public required bool HasParameterList { get; init; }
    public string[] ParameterNames { get; init; } = [];
    public int ParameterCount => ParameterNames.Length;
    public required TToken[] Body { get; init; }
}

public class CPreprocessorMacroExpansion<TToken>
    where TToken : ICPreprocessorToken
{
    public required CPreprocessorMacroDefinition<TToken> Definition { get; init; }
    public required TToken[][] Arguments { get; init; }

    public int BodyPosition { get; set; }
    // Set to -1 when not expanding an argument
    public int CurrentArgumentIndex { get; set; } = -1;
    public int ArgumentTokenIndex { get; set; }
}

public interface ICPreprocessorToken : ILexerToken
{
    public long CPreprocessorIntegerValue { get; }
    public ReadOnlySpan<char> CPreprocessorTextSpan => SourceSpan;
    public bool IsCPreprocessorMacroParameter { get; }
    public int CPreprocessorMacroParameterIndex { get; }
}

public interface ICPreprocessorLexer<TToken> : ILexer<TToken>
    where TToken : ICPreprocessorToken
{
    public void SkipCPreprocessorWhiteSpaceAndComments();
    public TToken ReadCPreprocessorToken();
    public TToken ReadCPreprocessorIdentifierToken();
    public TToken ReadCPreprocessorNumberToken();
}

public class CPreprocessor : CPreprocessor<CToken, CLexer>
{
    public CPreprocessor(CLexer lexer)
        : base(lexer)
    {
    }
}

public class CPreprocessor<TToken, TLexer>
    where TToken : ICPreprocessorToken
    where TLexer : ICPreprocessorLexer<TToken>
{
    public TLexer Lexer { get; }
    public LexerState LexerState => Lexer.State;
    public CPreprocessorState PreprocessorState { get; } = new();

    protected readonly Stack<CPreprocessorMacroExpansion<TToken>> MacroExpansions = new();

    public CPreprocessor(TLexer lexer)
    {
        Lexer = lexer;
    }

    public bool TryGetCurrentExpansionToken([MaybeNullWhen(false)] out TToken token)
    {
        if (MacroExpansions.Count == 0)
        {
            token = default!;
            return false;
        }

        var currentMacroExpansion = MacroExpansions.Peek();
        var currentMacroBody = currentMacroExpansion.Definition.Body;

        if (currentMacroExpansion.CurrentArgumentIndex >= 0)
        {
            var currentArgumentTokens = currentMacroExpansion.Arguments[currentMacroExpansion.CurrentArgumentIndex];

            token = currentArgumentTokens[currentMacroExpansion.ArgumentTokenIndex];
            currentMacroExpansion.ArgumentTokenIndex++;

            if (currentMacroExpansion.ArgumentTokenIndex >= currentArgumentTokens.Length)
            {
                currentMacroExpansion.CurrentArgumentIndex = -1;
                currentMacroExpansion.ArgumentTokenIndex = 0;

                if (currentMacroExpansion.BodyPosition >= currentMacroBody.Length)
                    MacroExpansions.Pop();
            }

            return true;
        }

        if (currentMacroExpansion.BodyPosition >= currentMacroBody.Length)
        {
            MacroExpansions.Pop();
            token = default!;
            return false;
        }

        Debug.Assert(currentMacroExpansion.BodyPosition >= 0);
        Debug.Assert(currentMacroExpansion.BodyPosition < currentMacroBody.Length);

        token = currentMacroBody[currentMacroExpansion.BodyPosition];
        currentMacroExpansion.BodyPosition++;

        if (token.IsCPreprocessorMacroParameter)
        {
            currentMacroExpansion.CurrentArgumentIndex = token.CPreprocessorMacroParameterIndex;
            currentMacroExpansion.ArgumentTokenIndex = 0;
            return TryGetCurrentExpansionToken(out token);
        }

        if (currentMacroExpansion.BodyPosition >= currentMacroBody.Length)
            MacroExpansions.Pop();

        return true;
    }

    public void ReadDirective()
    {
        Lexer.SkipCPreprocessorWhiteSpaceAndComments();

        if (CFacts.IsIdentifierStartCharacter(LexerState.CurrentCharacter))
        {
            var directiveToken = Lexer.ReadCPreprocessorIdentifierToken();

            if (MemoryExtensions.Equals(directiveToken.SourceSpan, "define", StringComparison.InvariantCulture))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new InternalCompilerException(directiveToken.SourceLocation, $"{nameof(CLexer)}.{nameof(ReadDirective)} did not recognize the preprocessor directive.");
            }
        }
        else if (CFacts.IsNumericLiteralDigit(LexerState.CurrentCharacter))
        {
            var lineNumberToken = Lexer.ReadCPreprocessorNumberToken();
            throw new NotImplementedException();
        }

        throw new InternalCompilerException(LexerState.CurrentLocation, $"{nameof(CLexer)}.{nameof(ReadDirective)} expected an identifier or a number to start a directive.");
    }
}
