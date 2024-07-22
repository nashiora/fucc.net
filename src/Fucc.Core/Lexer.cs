// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Fucc;

public readonly struct LexerStateSnapshot
{
    public readonly SourceText Source;
    public readonly int CurrentCharacterOffset;
    public readonly int CurrentCharacterLength;
    public readonly int CurrentCharacter;
    public readonly bool IsAtStartOfLine;
    public readonly bool IsAtStartOfLineIgnoringWhiteSpace;

    internal LexerStateSnapshot(SourceText source, int currentCharacterOffset, int currentCharacterLength, int currentCharacter, bool isAtStartOfLine, bool isAtStartOfLineIgnoringWhiteSpace)
    {
        Source = source;
        CurrentCharacterOffset = currentCharacterOffset;
        CurrentCharacterLength = currentCharacterLength;
        CurrentCharacter = currentCharacter;
        IsAtStartOfLine = isAtStartOfLine;
        IsAtStartOfLineIgnoringWhiteSpace = isAtStartOfLineIgnoringWhiteSpace;
    }
}

public sealed class LexerState
{
    public SourceText Source { get; }

    public SourceLocation CurrentLocation => new(Source, CurrentCharacterOffset, CurrentCharacterLength);
    public bool IsAtEnd => CurrentCharacterOffset >= Source.Text.Length;

    public int CurrentCharacterOffset { get; private set; }
    public int CurrentCharacterLength { get; private set; }
    public int CurrentCharacter { get; private set; }

    public bool IsAtStartOfLine { get; private set; } = true;
    public bool IsAtStartOfLineIgnoringWhiteSpace { get; private set; } = true;

    public LexerState(SourceText source)
    {
        Source = source;
        ReadCharacter();
    }

    public LexerStateSnapshot TakeSnapshot()
    {
        return new(Source, CurrentCharacterOffset, CurrentCharacterLength, CurrentCharacter, IsAtStartOfLine, IsAtStartOfLineIgnoringWhiteSpace);
    }

    public void RestoreFromSnapshot(LexerStateSnapshot snapshot)
    {
        if (Source != snapshot.Source)
            throw new ArgumentException("Lexer snapshot does not share the same source text.", nameof(snapshot));

        CurrentCharacterOffset = snapshot.CurrentCharacterOffset;
        CurrentCharacterLength = snapshot.CurrentCharacterLength;
        CurrentCharacter = snapshot.CurrentCharacter;
        IsAtStartOfLine = snapshot.IsAtStartOfLine;
        IsAtStartOfLineIgnoringWhiteSpace = snapshot.IsAtStartOfLineIgnoringWhiteSpace;
    }

    public void ReadCharacter()
    {
        CurrentCharacterLength = 0;
        CurrentCharacter = 0;

        if (CurrentCharacterOffset >= Source.Text.Length)
            return;

        char currentChar = Source.Text[CurrentCharacterOffset];
        if (!char.IsHighSurrogate(currentChar))
        {
            CurrentCharacter = currentChar;
            CurrentCharacterLength = 1;
            return;
        }

        if (CurrentCharacterOffset + 1 >= Source.Text.Length || !char.IsLowSurrogate(Source.Text[CurrentCharacterOffset + 1]))
        {
            CurrentCharacter = '?';
            CurrentCharacterLength = 1;
        }
        else
        {
            CurrentCharacter = char.ConvertToUtf32(currentChar, Source.Text[CurrentCharacterOffset + 1]);
            CurrentCharacterLength = 2;
        }
    }

    public void Advance()
    {
        if (CurrentCharacter == '\n')
        {
            IsAtStartOfLine = true;
            IsAtStartOfLineIgnoringWhiteSpace = true;
        }
        else
        {
            IsAtStartOfLine = false;
            if (IsAtStartOfLineIgnoringWhiteSpace && !char.IsWhiteSpace((char)CurrentCharacter))
            {
                IsAtStartOfLineIgnoringWhiteSpace = false;
            }
        }

        CurrentCharacterOffset += CurrentCharacterLength;
        ReadCharacter();
    }

    public int PeekCharacter(int ahead = 1)
    {
        if (ahead < 0)
            throw new ArgumentOutOfRangeException(nameof(ahead), "Can only peek the current character or some number of characters ahead. Value for 'ahead' cannot be negative.");
        
        int currentCharacterOffset = CurrentCharacterOffset;
        int currentCharacterLength = CurrentCharacterLength;
        int currentCharacter = CurrentCharacter;
        bool isAtStartOfLine = IsAtStartOfLine;
        bool isAtStartOfLineIgnoringWhiteSpace = IsAtStartOfLineIgnoringWhiteSpace;

        for (int i = 0; i < ahead; i++)
            Advance();

        int peekedCharacter = CurrentCharacter;
        
        CurrentCharacterOffset = currentCharacterOffset;
        CurrentCharacterLength = currentCharacterLength;
        CurrentCharacter = currentCharacter;
        IsAtStartOfLine = isAtStartOfLine;
        IsAtStartOfLineIgnoringWhiteSpace = isAtStartOfLineIgnoringWhiteSpace;

        return peekedCharacter;
    }
}

public interface ILexerToken
{
    public SourceLocation SourceLocation { get; }
    public ReadOnlySpan<char> SourceSpan => SourceLocation.SourceSpan;

    public bool IsEndOfFileToken { get; }
}

public interface ILexer<TToken>
    where TToken : ILexerToken
{
    public LexerState State { get; }
    public TToken ReadToken();
}

public abstract class Lexer<TToken>(LexerState state) : ILexer<TToken>
    where TToken : ILexerToken
{
    public LexerState State { get; } = state;
    public abstract TToken ReadToken();

    public virtual void ReadCharacter() => State.ReadCharacter();
    public virtual void Advance() => State.Advance();
    public virtual int PeekCharacter(int ahead = 1) => State.PeekCharacter(ahead);
}
