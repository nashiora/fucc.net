namespace Fucc.FrontEnd.C;

public class CPreprocessorState
{
    public void InitializeFrom(CPreprocessorState other)
    {
    }
}

public class CPreprocessor
{
    public CLexer Lexer { get; }
    public LexerState LexerState => Lexer.State;
    public CPreprocessorState PreprocessorState { get; } = new();

    public CPreprocessor(CLexer lexer)
    {
        Lexer = lexer;
    }

    public void ReadDirective()
    {
        throw new NotImplementedException();
    }
}
