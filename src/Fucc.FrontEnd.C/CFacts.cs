// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Fucc.FrontEnd.C;

public static class CFacts
{
    public static bool IsIdentifierStartCharacter(char c) => IsIdentifierStartCharacter((int)c);
    public static bool IsIdentifierStartCharacter(int codepoint)
    {
        return codepoint == '_' || (codepoint >= 'a' && codepoint <= 'z') || (codepoint >= 'A' && codepoint <= 'Z');
    }
    
    public static bool IsIdentifierPartCharacter(char c) => IsIdentifierPartCharacter((int)c);
    public static bool IsIdentifierPartCharacter(int codepoint)
    {
        return IsIdentifierStartCharacter(codepoint) || (codepoint >= '0' && codepoint <= '9');
    }

    public static bool IsWhiteSpaceCharacter(char c) => IsWhiteSpaceCharacter((int)c);
    public static bool IsWhiteSpaceCharacter(int codepoint)
    {
        return codepoint == ' ' || codepoint == '\r' || codepoint == '\n' || codepoint == '\v' || codepoint == '\t';
    }

    public static bool IsNumericLiteralDigit(char c) => IsNumericLiteralDigit((int)c);
    public static bool IsNumericLiteralDigit(int codepoint)
    {
        return codepoint >= '0' && codepoint <= '9';
    }

    public static int NumericLiteralDigitValue(char c) => NumericLiteralDigitValue((int)c);
    public static int NumericLiteralDigitValue(int codepoint)
    {
        if (!IsNumericLiteralDigit(codepoint))
            throw new ArgumentException($"{nameof(NumericLiteralDigitValue)} can only accept a character/codepoint that is a valid numeric literal digit.", nameof(codepoint));

        return codepoint - '0';
    }

    public static bool IsNumericLiteralDigitInRadix(char c, int radix) => IsNumericLiteralDigitInRadix((int)c, radix);
    public static bool IsNumericLiteralDigitInRadix(int codepoint, int radix)
    {
        if (radix < 2 || radix > 36)
            throw new ArgumentException($"{nameof(IsNumericLiteralDigit)} can only accept a radix in the range [2, 36].", nameof(radix));

        if (radix <= 10)
            return codepoint >= '0' && codepoint <= ('0' + radix);
        
        return (codepoint >= '0' && codepoint <= '9') || (codepoint >= 'a' && codepoint <= 'a' + (radix - 10)) || (codepoint >= 'A' && codepoint <= 'A' + (radix - 10));
    }

    public static int NumericLiteralDigitValueInRadix(char c, int radix) => NumericLiteralDigitValueInRadix((int)c, radix);
    public static int NumericLiteralDigitValueInRadix(int codepoint, int radix)
    {
        if (radix < 2 || radix > 36)
            throw new ArgumentException($"{nameof(NumericLiteralDigitValueInRadix)} can only accept a radix in the range [2, 36].", nameof(radix));

        if (!IsNumericLiteralDigitInRadix(codepoint, radix))
            throw new ArgumentException($"{nameof(NumericLiteralDigitValue)} can only accept a character/codepoint that is a valid numeric literal digit in base {radix}.", nameof(codepoint));

        if (codepoint >= '0' && codepoint <= '9')
            return codepoint - '0';
            
        if (codepoint >= 'a' && codepoint <= 'z')
            return codepoint - 'a';

        Debug.Assert(codepoint >= 'A' && codepoint <= 'Z');
        return codepoint - 'A';
    }
}
