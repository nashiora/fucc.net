// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Fucc;

public readonly record struct FuccTarget
{
    public readonly Size SizeOfPointer;
    public readonly Align AlignOfPointer;
}

public sealed class IRTypeStorage
{
    private readonly Dictionary<int, FuccIRType.Integer> _integerTypes = [];

    public FuccContext Context { get; }

    public FuccIRType.Void Void { get; }

    public FuccIRType.Integer PointerWidthInteger { get; }

    public FuccIRType.Float Single { get; }
    public FuccIRType.Float Double { get; }
    public FuccIRType.Float Float80 { get; }
    public FuccIRType.Float Quad { get; }

    internal IRTypeStorage(FuccContext context)
    {
        Context = context;

        Void = new(context);

        PointerWidthInteger = Integer(Context.Target.SizeOfPointer.Bits);

        Single = new(context, 32);
        Double = new(context, 64);
        Float80 = new(context, 80);
        Quad = new(context, 128);
    }

    public FuccIRType.Integer Integer(int sizeInBits)
    {
        Debug.Assert(sizeInBits > 0 && sizeInBits <= ushort.MaxValue, "Integer bit width must be in the range (0, 65535].");

        if (!_integerTypes.TryGetValue(sizeInBits, out var type))
            _integerTypes[sizeInBits] = type = new FuccIRType.Integer(Context, sizeInBits);

        return type;
    }
}

public class FuccContext
{
    public required FuccTarget Target { get; init; }

    public IRTypeStorage TypeStorage { get; }

    public FuccContext()
    {
        TypeStorage = new(this);
    }
}
