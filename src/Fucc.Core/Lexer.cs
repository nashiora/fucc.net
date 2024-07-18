namespace Fucc;

public sealed class LexerState
{
    public SourceText Source { get; }

    public SourceLocation CurrentLocation => new(Source, CurrentCharacterOffset, CurrentCharacterLength);
    public bool IsAtEnd => CurrentCharacterOffset >= Source.Text.Length;

    public int CurrentCharacterOffset { get; private set; }
    public int CurrentCharacterLength { get; private set; }
    public int CurrentCharacter { get; private set; }

    public bool IsAtStartOfLine { get; private set; } = true;

    public LexerState(SourceText source)
    {
        Source = source;
        ReadCharacter();
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
        IsAtStartOfLine = CurrentCharacter == '\n';

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

        for (int i = 0; i < ahead; i++)
            Advance();

        int peekedCharacter = CurrentCharacter;
        
        CurrentCharacterOffset = currentCharacterOffset;
        CurrentCharacterLength = currentCharacterLength;
        CurrentCharacter = currentCharacter;
        IsAtStartOfLine = isAtStartOfLine;

        return peekedCharacter;
    }
}

public interface ILexer<TToken>
{
    public LexerState State { get; }
    public TToken ReadToken();
}

public abstract class Lexer<TToken>(LexerState state) : ILexer<TToken>
{
    public LexerState State { get; } = state;
    public abstract TToken ReadToken();

    public virtual void ReadCharacter() => State.ReadCharacter();
    public virtual void Advance() => State.Advance();
    public virtual int PeekCharacter(int ahead = 1) => State.PeekCharacter(ahead);
}
