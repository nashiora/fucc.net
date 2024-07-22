// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Fucc;

public sealed class SourceText : IEquatable<SourceText>
{
    private static int _idCounter = 0;
    
    public static readonly SourceText Unknown = new("<unknown>", "", "");

    public static bool operator ==(SourceText? a, SourceText? b) => a is null ? b is null : a.Equals(b);
    public static bool operator !=(SourceText? a, SourceText? b) => !(a == b);

    public static SourceText CreateFromText(string name, string text)
    {
        return new(name, "", text);
    }

    public static SourceText CreateFromFile(string filePath)
    {
        string name = System.IO.Path.GetFileName(filePath);
        string text = File.ReadAllText(filePath);
        return new(name, filePath, text);
    }

    public static SourceText CreateFromFile(FileInfo file)
    {
        return CreateFromFile(file.FullName);
    }

    public int Id { get; }

    public string Name { get; }
    public string Path { get; }

    public string Text { get; }

    private SourceText(string name, string path, string text)
    {
        Id = Interlocked.Increment(ref _idCounter);

        Name = name;
        Path = path;
        Text = text;
    }

    public SourceLocation Location(int offset, int length) => new(this, offset, length);
    public ReadOnlySpan<char> GetSpan(int offset, int length) => Text.AsSpan().Slice(offset, length);

    public override int GetHashCode() => Id;

    public override bool Equals(object? obj) => obj is SourceText that && Equals(that);
    public bool Equals(SourceText? that) => that is not null && Id == that.Id;
}

public readonly record struct SourceLocation
{
    public static readonly SourceLocation Unknown = new(SourceText.Unknown, 0, 0);

    public readonly SourceText SourceText;
    public readonly int Offset;
    public readonly int Length;

    public ReadOnlySpan<char> SourceSpan => SourceText.GetSpan(Offset, Length);

    public SourceLocation(SourceText sourceText, int offset, int length)
    {
        SourceText = sourceText;
        Offset = Math.Max(0, offset);
        Length = Math.Max(0, length);
    }

    public SourceLocation UpTo(SourceLocation latter)
    {
        if (SourceText != latter.SourceText)
            throw new ArgumentException("The two source locations should be part of the same source text.");
            
        if (Offset > latter.Offset)
            throw new ArgumentException("The latter source location must have an offset greater or equal to this.");

        return new(SourceText, Offset, latter.Offset - Offset);
    }

    public SourceLocation CombineRange(SourceLocation other)
    {
        if (SourceText != other.SourceText)
            throw new ArgumentException("The two source locations should be part of the same source text.");

        int offset = Math.Min(Offset, other.Offset);
        return new(SourceText, offset, Math.Max(Offset + Length, other.Offset + other.Length) - offset);
    }
}
