namespace Fucc.FrontEnd.C;

public class CLexer : Lexer<CToken>
{
    public CPreprocessor Preprocessor { get; }

    public CLexer(LexerState state)
        : base(state)
    {
        Preprocessor = new(this);
    }

    public override CToken ReadToken()
    {
        throw new NotImplementedException();
    }

    public CToken ReadIdentifierToken()
    {
        if (!CFacts.IsIdentifierStartCharacter(State.CurrentCharacter))
            throw new InternalCompilerException(State.CurrentLocation, $"{nameof(CLexer)}.{nameof(ReadIdentifierToken)} was called when not at a valid C identifier start character.");
            
        throw new NotImplementedException();
    }
}
