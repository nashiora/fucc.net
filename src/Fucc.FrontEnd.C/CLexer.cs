// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Fucc.FrontEnd.C;

public class CLexer : Lexer<CToken>, ICPreprocessorLexer<CToken>
{
    public CPreprocessor Preprocessor { get; }

    public CLexer(LexerState state)
        : base(state)
    {
        Preprocessor = new(this);
    }

    CToken ICPreprocessorLexer<CToken>.ReadCPreprocessorToken() => ReadToken();
    public override CToken ReadToken()
    {
        if (Preprocessor.TryGetCurrentExpansionToken(out var expansionToken))
            return expansionToken;
        
        SkipWhiteSpaceAndComments();

        if (State.IsAtStartOfLineIgnoringWhiteSpace && State.CurrentCharacter == '#')
        {
            Advance();
            Preprocessor.ReadDirective();
            SkipWhiteSpaceAndComments();
        }

        var token = ReadTokenNoKeywordsOrPreprocessing();
        if (token.SyntaxKind == CSyntaxKind.TokenIdentifier)
        {
            // look up macro definitions

        ident_is_not_a_macro:;
            // change to a keyword if it is one
        }

        return token;
    }

    public CToken ReadTokenNoKeywordsOrPreprocessing()
    {
        SkipWhiteSpaceAndComments();

        if (State.IsAtEnd)
            return new(CSyntaxKind.TokenEndOfFile, State.CurrentLocation);

        int currentCharacter = State.CurrentCharacter;
        var startLocation = State.CurrentLocation;

        switch (currentCharacter)
        {
            case '(': case ')':
            case '[': case ']':
            case '{': case '}':
            case ',': case ';': case ':':
            {
                Advance();
                return new((CSyntaxKind)currentCharacter, startLocation);
            }
            
            case '.':
            {
                Advance();
                if (State.CurrentCharacter == '.' && PeekCharacter(1) == '.')
                {
                    Advance();
                    Advance();
                    return new(CSyntaxKind.TokenDotDotDot, startLocation.UpTo(State.CurrentLocation));
                }
                else return new((CSyntaxKind)currentCharacter, startLocation);
            }
            
            case '+':
            {
                Advance();
                if (State.CurrentCharacter == '+')
                {
                    Advance();
                    return new(CSyntaxKind.TokenPlusPlus, startLocation.UpTo(State.CurrentLocation));
                }
                else if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(CSyntaxKind.TokenPlusEqual, startLocation.UpTo(State.CurrentLocation));
                }
                else return new((CSyntaxKind)currentCharacter, startLocation);
            }
            
            case '-':
            {
                Advance();
                if (State.CurrentCharacter == '-')
                {
                    Advance();
                    return new(CSyntaxKind.TokenMinusMinus, startLocation.UpTo(State.CurrentLocation));
                }
                else if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(CSyntaxKind.TokenMinusEqual, startLocation.UpTo(State.CurrentLocation));
                }
                else return new((CSyntaxKind)currentCharacter, startLocation);
            }
            
            case '*':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(CSyntaxKind.TokenStarEqual, startLocation.UpTo(State.CurrentLocation));
                }
                else return new((CSyntaxKind)currentCharacter, startLocation);
            }
            
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
            {
                return ReadNumberToken();
            }

            default:
            {
                if (CFacts.IsIdentifierStartCharacter(currentCharacter))
                    return ReadIdentifierToken();
                
                throw new InternalCompilerException(State.CurrentLocation, $"{nameof(CLexer)} encountered an unknown character (with value {State.CurrentCharacter}) in the source text, aborting.");
            }
        }
    }

    void ICPreprocessorLexer<CToken>.SkipCPreprocessorWhiteSpaceAndComments() => SkipWhiteSpaceAndComments();
    public void SkipWhiteSpaceAndComments()
    {
        while (!State.IsAtEnd)
        {
            if (CFacts.IsWhiteSpaceCharacter(State.CurrentCharacter))
            {
                Advance();
            }
            else if (State.CurrentCharacter == '\\' && PeekCharacter(1) == '\n')
            {
                Advance();
                Advance();
            }
            else if (State.CurrentCharacter == '/' && PeekCharacter() == '/')
            {
                // TODO(local): backslash+newline in comments
                while (!State.IsAtEnd && State.CurrentCharacter != '\n')
                    Advance();
            }
            else if (State.CurrentCharacter == '/' && PeekCharacter() == '*')
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
                    else previousCharacter = currentCharacter;
                }

                if (nestingDepth > 0)
                {
                    throw new InternalCompilerException(startLocation, $"{nameof(CLexer)} encountered the end of the source text while reading a (potentially nested) delimited comment. Diagnostic reporting is not yet supported.");
                }
            }
            else break;
        }
    }

    CToken ICPreprocessorLexer<CToken>.ReadCPreprocessorIdentifierToken() => ReadIdentifierToken();
    public CToken ReadIdentifierToken()
    {
        if (!CFacts.IsIdentifierStartCharacter(State.CurrentCharacter))
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(CLexer)}.{nameof(ReadIdentifierToken)} was called when not at a valid C identifier start character.");
            
        var location = State.CurrentLocation;
        Advance();

        while (!State.IsAtEnd && CFacts.IsIdentifierPartCharacter(State.CurrentCharacter))
        {
            Advance();
        }

        location = location.UpTo(State.CurrentLocation);
        return new(CSyntaxKind.TokenIdentifier, location);
    }

    CToken ICPreprocessorLexer<CToken>.ReadCPreprocessorNumberToken() => ReadNumberToken();
    public CToken ReadNumberToken()
    {
        if (!CFacts.IsNumericLiteralDigit(State.CurrentCharacter))
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(CLexer)}.{nameof(ReadNumberToken)} was called when not at a valid numeric literal digit character.");
        
        var location = State.CurrentLocation;

        long primaryNumberValue = 0;
        while (!State.IsAtEnd && CFacts.IsNumericLiteralDigit(State.CurrentCharacter))
        {
            int digitValue = CFacts.NumericLiteralDigitValue(State.CurrentCharacter);
            Debug.Assert(digitValue >= 0 && digitValue <= 9);

            Advance();

            if (long.MaxValue - primaryNumberValue < digitValue)
                throw new InternalCompilerException(State.CurrentLocation, $"{nameof(CLexer)}.{nameof(ReadNumberToken)} doesn't gracefully handle literals with a value larger than {long.MaxValue} currently.");

            primaryNumberValue = primaryNumberValue * 10 + digitValue;
        }

        location = location.UpTo(State.CurrentLocation);
        return new(CSyntaxKind.TokenLiteralInteger, location)
        {
            IntegerValue = primaryNumberValue,
        };
    }
}
