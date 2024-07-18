namespace Fucc.FrontEnd.Laye;

public class LayeToken : LayeSyntax
{
    public LayeSyntaxKind ContextualKind { get; internal set; } = LayeSyntaxKind.Invalid;

    public LayeToken(LayeSyntaxKind syntaxKind, SourceLocation sourceLocation)
        : base(syntaxKind, sourceLocation)
    {
    }
}
