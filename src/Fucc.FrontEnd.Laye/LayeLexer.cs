using Fucc.FrontEnd.C;

namespace Fucc.FrontEnd.Laye;

public class LayeLexer : Lexer<LayeToken>
{
    private static readonly (string Image, LayeSyntaxKind Kind)[] _keywordTokensKinds =
    [
        ("var", LayeSyntaxKind.TokenVar)
    ];

    public CLexer CLexer { get; }
    public CPreprocessor CPreprocessor => CLexer.Preprocessor;

    public LayeLexer(LexerState state)
        : base(state)
    {
        CLexer = new(state);
    }

    public override LayeToken ReadToken()
    {
        SkipWhiteSpaceAndComments();

        if (State.IsAtEnd)
            return new(LayeSyntaxKind.TokenEndOfFile, State.CurrentLocation);

        switch (State.CurrentCharacter)
        {
            default:
            {
                if (LayeFacts.IsIdentifierStartCharacter(State.CurrentCharacter))
                    return ReadIdentifierOrKeywordToken();
                
                throw new InternalCompilerException(State.CurrentLocation, $"{nameof(LayeLexer)} encountered an unknown character (with value {State.CurrentCharacter}) in the source text, aborting.");
            }
        }
    }

    public void SkipWhiteSpaceAndComments()
    {
        while (!State.IsAtEnd)
        {
            if (LayeFacts.IsWhiteSpaceCharacter(State.CurrentCharacter))
            {
                Advance();
                continue;
            }

            if (State.CurrentCharacter == '/' && PeekCharacter() == '/')
            {
                while (!State.IsAtEnd && State.CurrentCharacter != '\n')
                    Advance();
                continue;
            }

            if (State.CurrentCharacter == '/' && PeekCharacter() == '*')
            {
                var startLocation = State.CurrentLocation;

                Advance();
                Advance();

                int nestingDepth = 1;
                int previousCharacter = 0;

                while (nestingDepth > 0 && !State.IsAtEnd)
                {
                    int currentCharacter = State.CurrentCharacter;
                    Advance();

                    if (previousCharacter == '*' && currentCharacter == '/')
                    {
                        nestingDepth--;
                        previousCharacter = 0;
                    }
                    else if (previousCharacter == '/' && currentCharacter == '*')
                    {
                        nestingDepth++;
                        previousCharacter = 0;
                    }
                    else previousCharacter = currentCharacter;
                }

                if (nestingDepth > 0)
                {
                    throw new InternalCompilerException(startLocation, $"{nameof(LayeLexer)} encountered the end of the source text while reading a (potentially nested) delimited comment. Diagnostic reporting is not yet supported.");
                }

                continue;
            }

            break;
        }
    }

    public LayeToken ReadIdentifierOrKeywordToken()
    {
        if (!LayeFacts.IsIdentifierStartCharacter(State.CurrentCharacter))
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(LayeLexer)}.{nameof(ReadIdentifierOrKeywordToken)} was called when not at a valid Laye identifier start character.");

        var eitherToken = ReadIdentifierToken();

        var tokenSourceText = eitherToken.SourceLocation.SourceSpan;
        foreach (var (image, keywordKind) in _keywordTokensKinds)
        {
            if (MemoryExtensions.Equals(tokenSourceText, image, StringComparison.InvariantCulture))
            {
                eitherToken.SyntaxKind = keywordKind;
                break;
            }
        }
        
        return eitherToken;
    }

    public LayeToken ReadIdentifierToken()
    {
        if (!LayeFacts.IsIdentifierStartCharacter(State.CurrentCharacter))
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(LayeLexer)}.{nameof(ReadIdentifierToken)} was called when not at a valid Laye identifier start character.");
            
        var location = State.CurrentLocation;
        Advance();

        while (!State.IsAtEnd && LayeFacts.IsIdentifierPartCharacter(State.CurrentCharacter))
        {
            Advance();
        }

        location = location.UpTo(State.CurrentLocation);
        return new(LayeSyntaxKind.TokenIdentifier, location);
    }
}
