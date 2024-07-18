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
}
