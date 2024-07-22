// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Fucc.FrontEnd.C;

public class CSyntax
{
    public CSyntaxKind SyntaxKind { get; internal set; }
    public SourceLocation SourceLocation { get; internal set; }

    public CSyntax(CSyntaxKind syntaxKind, SourceLocation sourceLocation)
    {
        SyntaxKind = syntaxKind;
        SourceLocation = sourceLocation;
    }
}

public class CToken : CSyntax, ILexerToken, ICPreprocessorToken
{
    public CSyntaxKind ContextualKind { get; internal set; } = CSyntaxKind.Invalid;

    long ICPreprocessorToken.CPreprocessorIntegerValue => IntegerValue;
    public long IntegerValue { get; internal set; }
    public double FloatValue { get; internal set; }
    public string StringValue { get; internal set; } = string.Empty;

    public bool IsCPreprocessorMacroParameter { get; set; }
    public int CPreprocessorMacroParameterIndex { get; set; }

    bool ILexerToken.IsEndOfFileToken => SyntaxKind == CSyntaxKind.TokenEndOfFile;

    public CToken(CSyntaxKind syntaxKind, SourceLocation sourceLocation)
        : base(syntaxKind, sourceLocation)
    {
    }

    public override string ToString() => $"{SyntaxKind} :: {SourceLocation.SourceSpan}";
}

public enum CSyntaxKind
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
    TokenLiteralInteger,
    
    TokenAuto,
    TokenBreak,
    TokenCase,
    TokenChar,
    TokenConst,
    TokenContinue,
    TokenDefault,
    TokenDo,
    TokenDouble,
    TokenElse,
    TokenEnum,
    TokenExtern,
    TokenFloat,
    TokenFor,
    TokenGoto,
    TokenIf,
    TokenInt,
    TokenLong,
    TokenRegister,
    TokenReturn,
    TokenShort,
    TokenSigned,
    TokenSizeof,
    TokenStatic,
    TokenStruct,
    TokenSwitch,
    TokenTypedef,
    TokenUnion,
    TokenUnsigned,
    TokenVoid,
    TokenVolatile,
    TokenWhile,
    TokenDotDotDot,
    TokenPlusPlus,
    TokenPlusEqual,
    TokenMinusMinus,
    TokenMinusEqual,
    TokenStarEqual,
    TokenSlashEqual,
    TokenPercentEqual,
    TokenAmpersandAmpersand,
    TokenAmpersandEqual,
    TokenPipePipe,
    TokenPipeEqual,
    TokenCaretEqual,
    TokenLessLessEqual,
    TokenLessLess,
    TokenLessEqual,
    TokenGreaterGreaterEqual,
    TokenGreaterGreater,
    TokenGreaterEqual,
    TokenEqualEqual,
    TokenBangEqual,
}
