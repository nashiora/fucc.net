using System.Text;
using Fucc.FrontEnd.C;

namespace Fucc.FrontEnd.Laye;

public class LayeLexer : Lexer<LayeToken>
{
    private static readonly (string Image, LayeSyntaxKind Kind)[] _keywordTokensKinds =
    [
        ("var", LayeSyntaxKind.TokenVar),
        ("void", LayeSyntaxKind.TokenVoid),
        ("noreturn", LayeSyntaxKind.TokenNoReturn),
        ("bool", LayeSyntaxKind.TokenBool),
        ("int", LayeSyntaxKind.TokenInt),
        ("true", LayeSyntaxKind.TokenTrue),
        ("false", LayeSyntaxKind.TokenFalse),
        ("nil", LayeSyntaxKind.TokenNil),
        ("if", LayeSyntaxKind.TokenIf),
        ("else", LayeSyntaxKind.TokenElse),
        ("for", LayeSyntaxKind.TokenFor),
        ("while", LayeSyntaxKind.TokenWhile),
        ("do", LayeSyntaxKind.TokenDo),
        ("switch", LayeSyntaxKind.TokenSwitch),
        ("case", LayeSyntaxKind.TokenCase),
        ("default", LayeSyntaxKind.TokenDefault),
        ("return", LayeSyntaxKind.TokenReturn),
        ("break", LayeSyntaxKind.TokenBreak),
        ("continue", LayeSyntaxKind.TokenContinue),
        ("fallthrough", LayeSyntaxKind.TokenFallthrough),
        ("yield", LayeSyntaxKind.TokenYield),
        ("unreachable", LayeSyntaxKind.TokenUnreachable),
        ("defer", LayeSyntaxKind.TokenDefer),
        ("discard", LayeSyntaxKind.TokenDiscard),
        ("goto", LayeSyntaxKind.TokenGoto),
        ("xyzzy", LayeSyntaxKind.TokenXyzzy),
        ("assert", LayeSyntaxKind.TokenAssert),
        ("try", LayeSyntaxKind.TokenTry),
        ("catch", LayeSyntaxKind.TokenCatch),
        ("struct", LayeSyntaxKind.TokenStruct),
        ("variant", LayeSyntaxKind.TokenVariant),
        ("enum", LayeSyntaxKind.TokenEnum),
        ("alias", LayeSyntaxKind.TokenAlias),
        ("template", LayeSyntaxKind.TokenTemplate),
        ("test", LayeSyntaxKind.TokenTest),
        ("import", LayeSyntaxKind.TokenImport),
        ("export", LayeSyntaxKind.TokenExport),
        ("operator", LayeSyntaxKind.TokenOperator),
        ("mut", LayeSyntaxKind.TokenMut),
        ("new", LayeSyntaxKind.TokenNew),
        ("delete", LayeSyntaxKind.TokenDelete),
        ("cast", LayeSyntaxKind.TokenCast),
        ("is", LayeSyntaxKind.TokenIs),
        ("sizeof", LayeSyntaxKind.TokenSizeof),
        ("alignof", LayeSyntaxKind.TokenAlignof),
        ("offsetof", LayeSyntaxKind.TokenOffsetof),
        ("not", LayeSyntaxKind.TokenNot),
        ("and", LayeSyntaxKind.TokenAnd),
        ("or", LayeSyntaxKind.TokenOr),
        ("xor", LayeSyntaxKind.TokenXor),
        ("varargs", LayeSyntaxKind.TokenVarargs),
        ("const", LayeSyntaxKind.TokenConst),
        ("foreign", LayeSyntaxKind.TokenForeign),
        ("inline", LayeSyntaxKind.TokenInline),
        ("callconv", LayeSyntaxKind.TokenCallconv),
        ("pure", LayeSyntaxKind.TokenPure),
        ("discardable", LayeSyntaxKind.TokenDiscardable),
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

        int currentCharacter = State.CurrentCharacter;
        var startLocation = State.CurrentLocation;

        switch (currentCharacter)
        {
            case '(': case ')':
            case '[': case ']':
            case '{': case '}':
            case ',': case ';': case '.':
            {
                Advance();
                return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case ':':
            {
                Advance();
                if (State.CurrentCharacter == ':')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenColonColon, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '~':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenTildeEqual, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '!':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenBangEqual, startLocation);
                }
                else if (State.CurrentCharacter == '.')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenBangDot, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '?':
            {
                Advance();
                if (State.CurrentCharacter == '?')
                {
                    Advance();
                    if (State.CurrentCharacter == '=')
                    {
                        Advance();
                        return new(LayeSyntaxKind.TokenQuestionQuestionEqual, startLocation);
                    }
                    else return new(LayeSyntaxKind.TokenQuestionQuestion, startLocation);
                }
                else if (State.CurrentCharacter == '.')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenQuestionDot, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '%':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenPercentEqual, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '&':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenAmpersandEqual, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '|':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenPipeEqual, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '+':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenPlusEqual, startLocation);
                }
                else if (State.CurrentCharacter == '+')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenPlusPlus, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '-':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenMinusEqual, startLocation);
                }
                else if (State.CurrentCharacter == '-')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenMinusMinus, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '=':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenEqualEqual, startLocation);
                }
                else if (State.CurrentCharacter == '>')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenEqualGreater, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '/':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenSlashEqual, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '<':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenLessEqual, startLocation);
                }
                else if (State.CurrentCharacter == '<')
                {
                    Advance();
                    if (State.CurrentCharacter == '=')
                    {
                        Advance();
                        return new(LayeSyntaxKind.TokenLessLessEqual, startLocation);
                    }
                    else return new(LayeSyntaxKind.TokenLessLess, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '>':
            {
                Advance();
                if (State.CurrentCharacter == '=')
                {
                    Advance();
                    return new(LayeSyntaxKind.TokenGreaterEqual, startLocation);
                }
                else if (State.CurrentCharacter == '>')
                {
                    Advance();
                    if (State.CurrentCharacter == '=')
                    {
                        Advance();
                        return new(LayeSyntaxKind.TokenGreaterGreaterEqual, startLocation);
                    }
                    else return new(LayeSyntaxKind.TokenGreaterGreater, startLocation);
                }
                else return new((LayeSyntaxKind)currentCharacter, startLocation);
            }
            
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
            {
                return ReadNumberOrIdentifierToken();
            }

            case '@':
                throw new NotImplementedException();

            case '"':
            case '\'':
            {
                return ReadStringOrRuneLiteralToken();
            }

            default:
            {
                if (LayeFacts.IsIdentifierStartCharacter(currentCharacter))
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
            }
            else if (State.CurrentCharacter == '/' && PeekCharacter() == '/')
            {
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
            }
            else break;
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

    public LayeToken ReadNumberOrIdentifierToken()
    {
        throw new NotImplementedException();
    }

    // TODO(local): just split these two out into different functions, with a shared escape handler if possible
    public LayeToken ReadStringOrRuneLiteralToken()
    {
        if (State.CurrentCharacter != '"' && State.CurrentCharacter != '\'')
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(LayeLexer)}.{nameof(ReadStringOrRuneLiteralToken)} was called when not at a single or double quote character.");
        
        var location = State.CurrentLocation;
        int quoteCharacter = State.CurrentCharacter;
        bool isStringLiteral = quoteCharacter == '"';

        Advance();

        var builder = new StringBuilder(16);

        while (!State.IsAtEnd && State.CurrentCharacter != quoteCharacter)
        {
            int character = State.CurrentCharacter;
            Advance();

            if (character == '\\')
            {
                Advance();

                character = State.CurrentCharacter;
                switch (character)
                {
                    case '\\': Advance(); builder.Append('\\'); break;
                    case '"': Advance(); builder.Append('"'); break;
                    case '\'': Advance(); builder.Append('\''); break;
                    case 'a': Advance(); builder.Append('a'); break;
                    case 'b': Advance(); builder.Append('b'); break;
                    case 'f': Advance(); builder.Append('f'); break;
                    case 'n': Advance(); builder.Append('\n'); break;
                    case 'r': Advance(); builder.Append('r'); break;
                    case 't': Advance(); builder.Append('t'); break;
                    case 'v': Advance(); builder.Append('v'); break;
                    case '0': Advance(); builder.Append('\0'); break;

                    case 'x':
                    {
                        // TODO(local): Laye strings are actually UTF-8, so we need to be able to encode
                        // multiple \x escapes...
                        throw new NotImplementedException();
                    } break;

                    case 'u':
                    {
                        throw new NotImplementedException();
                    } break;

                    default:
                    {
                        throw new NotImplementedException();
                    } break;
                }

                if (!isStringLiteral) break; // just early exit for runes?
            }
            else if (character < 0x10000)
                builder.Append((char)character);
            else builder.Append(char.ConvertFromUtf32(character));
        }

        if (State.CurrentCharacter == quoteCharacter)
            Advance();
        else throw new InternalCompilerException(State.CurrentLocation, $"{nameof(LayeLexer)} encountered the end of a {(isStringLiteral ? "string" : "rune")} literal without a closing quotation.");

        location = location.UpTo(State.CurrentLocation);

        if (isStringLiteral)
        {
            return new(LayeSyntaxKind.TokenLiteralString, location)
            {
                StringValue = builder.ToString(),
            };
        }
        else
        {
            return new(LayeSyntaxKind.TokenLiteralRune, location)
            {
                IntegerValue = builder[0],
            };
        }
    }
}
