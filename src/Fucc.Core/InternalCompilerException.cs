namespace Fucc;

public class InternalCompilerException : Exception
{
    private static string FormatMessageWithLocation(SourceLocation location, string message)
    {
        return $"{location.SourceText.Path}:[{location.Offset}:{location.Length}]: Internal Compiler Exception: {message}";
    }

    public SourceLocation Location { get; }

    public InternalCompilerException(string message)
        : this(SourceLocation.Unknown, message)
    {
    }
    
    public InternalCompilerException(SourceLocation location, string message)
        : base(FormatMessageWithLocation(location, message))
    {
        Location = location;
    }
}
