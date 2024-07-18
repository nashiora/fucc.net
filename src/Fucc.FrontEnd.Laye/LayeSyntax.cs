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

    TokenPlusPlus,
    TokenMinusMinus,
    TokenLessLess,
    TokenGreaterGreater,
    TokenEqualEqual,
    TokenBangEqual,
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
    TokenImpure,
    TokenDiscardable,
}
