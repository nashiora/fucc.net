namespace Fucc.FrontEnd.Laye;

public class LayeSyntax
{
    public LayeSyntaxKind SyntaxKind { get; internal set; }
    public SourceLocation SourceLocation { get; internal set; }

    public LayeSyntax(LayeSyntaxKind syntaxKind, SourceLocation sourceLocation)
    {
        SyntaxKind = syntaxKind;
        SourceLocation = sourceLocation;
    }
}

public class LayeToken : LayeSyntax
{
    public LayeSyntaxKind ContextualKind { get; internal set; } = LayeSyntaxKind.Invalid;

    public long IntegerValue { get; internal set; }
    public double FloatValue { get; internal set; }
    public string StringValue { get; internal set; } = string.Empty;

    public LayeToken(LayeSyntaxKind syntaxKind, SourceLocation sourceLocation)
        : base(syntaxKind, sourceLocation)
    {
    }
}

public enum LayeSyntaxKind
{
    Invalid = 0,

    __PrintableTokenStart__ = 32,

    TokenTilde = '~',
    TokenBang = '!',
    TokenPercent = '%',
    TokenAmpersand = '&',
    TokenStar = '*',
    TokenOpenParen = '(',
    TokenCloseParen = ')',
    TokenMinus = '-',
    TokenEqual = '=',
    TokenPlus = '+',
    TokenOpenBracket = '[',
    TokenCloseBracket = ']',
    TokenOpenBrace = '{',
    TokenCloseBrace = '}',
    TokenPipe = '|',
    TokenSemiColon = ';',
    TokenColon = ':',
    TokenComma = ',',
    TokenLess = '<',
    TokenGreater = '>',
    TokenDot = '.',
    TokenSlash = '/',
    TokenQuestion = '?',
    
    __PrintableTokenEnd__ = 128,
    
    __MultiByteStart__ = 256,
    
    TokenUnknown,
    TokenEndOfFile,

    TokenIdentifier,
    TokenGlobal,

    TokenLiteralInteger,
    TokenLiteralFloat,
    TokenLiteralString,
    TokenLiteralRune,

    TokenQuestionDot,
    TokenQuestionQuestion,
    TokenQuestionQuestionEqual,
    TokenPlusPlus,
    TokenMinusMinus,
    TokenLessLess,
    TokenGreaterGreater,
    TokenEqualEqual,
    TokenBangEqual,
    TokenBangDot,
    TokenPlusEqual,
    TokenMinusEqual,
    TokenSlashEqual,
    TokenStarEqual,
    TokenPercentEqual,
    TokenLessEqual,
    TokenGreaterEqual,
    TokenAmpersandEqual,
    TokenPipeEqual,
    TokenTildeEqual,
    TokenLessLessEqual,
    TokenGreaterGreaterEqual,
    TokenEqualGreater,
    TokenLessMinus,
    TokenColonColon,

    TokenVar,
    TokenVoid,
    TokenNoReturn,
    TokenBool,
    TokenBoolSized,
    TokenInt,
    TokenIntSized,
    TokenFloatSized,

    TokenTrue,
    TokenFalse,
    TokenNil,

    TokenIf,
    TokenElse,
    TokenFor,
    TokenWhile,
    TokenDo,
    TokenSwitch,
    TokenCase,
    TokenDefault,
    TokenReturn,
    TokenBreak,
    TokenContinue,
    TokenFallthrough,
    TokenYield,
    TokenUnreachable,

    TokenDefer,
    TokenDiscard,
    TokenGoto,
    TokenXyzzy,
    TokenAssert,
    TokenTry,
    TokenCatch,

    TokenStruct,
    TokenVariant,
    TokenEnum,
    //TokenStrict,
    TokenTemplate,
    TokenAlias,
    TokenTest,
    TokenImport,
    TokenExport,
    //TokenFrom,
    //TokenAs,
    TokenOperator,

    TokenMut,
    TokenNew,
    TokenDelete,
    TokenCast,
    TokenIs,
    
    TokenSizeof,
    TokenAlignof,
    TokenOffsetof,

    TokenNot,
    TokenAnd,
    TokenOr,
    TokenXor,

    TokenVarargs,
    TokenConst,
    TokenForeign,
    TokenInline,
    TokenCallconv,
    TokenPure,
    TokenDiscardable,
}

