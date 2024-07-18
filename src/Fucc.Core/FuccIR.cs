using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Fucc;

public enum FuccIRTypeKind
{
    Void,
    Noreturn,
    Pointer,
    Bool,
    Integer,
    Float,
    Function,
    Struct,
}

public enum FuccIRValueKind
{
    Global,
    Function,
}

public enum FuccConstantKind
{
    Integer,
    Real,
    String,
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct FuccConstant
{
    private static string StringToLiteral(string input) {
        var literal = new System.Text.StringBuilder(input.Length + 2);
        literal.Append('"');
        foreach (var c in input) {
            switch (c) {
                case '\"': literal.Append("\\\""); break;
                case '\\': literal.Append(@"\\"); break;
                case '\0': literal.Append(@"\0"); break;
                case '\a': literal.Append(@"\a"); break;
                case '\b': literal.Append(@"\b"); break;
                case '\f': literal.Append(@"\f"); break;
                case '\n': literal.Append(@"\n"); break;
                case '\r': literal.Append(@"\r"); break;
                case '\t': literal.Append(@"\t"); break;
                case '\v': literal.Append(@"\v"); break;
                default:
                {
                    if (c >= 0x20 && c <= 0x7e)
                        literal.Append(c);
                    else literal.Append(@"\u").Append(((int)c).ToString("x4"));
                } break;
            }
        }
        literal.Append('"');
        return literal.ToString();
    }

    [FieldOffset(0)]
    public readonly FuccConstantKind Kind;

    [FieldOffset(sizeof(FuccConstantKind))]
    public readonly long IntegerValue;

    [FieldOffset(sizeof(FuccConstantKind))]
    public readonly double RealValue;

    [FieldOffset(sizeof(FuccConstantKind))]
    public readonly string StringValue = null!;

    public FuccConstant(long integerValue)
    {
        Kind = FuccConstantKind.Integer;
        IntegerValue = integerValue;
    }

    public FuccConstant(double realValue)
    {
        Kind = FuccConstantKind.Real;
        RealValue = realValue;
    }

    public FuccConstant(string stringValue)
    {
        Kind = FuccConstantKind.String;
        StringValue = stringValue;
    }

    public override string ToString()
    {
        return Kind switch
        {
            FuccConstantKind.Integer => IntegerValue.ToString(),
            FuccConstantKind.Real => RealValue.ToString(),
            FuccConstantKind.String => StringToLiteral(StringValue),
            _ => throw new UnreachableException(),
        };
    }
}

public readonly struct Align : IEquatable<Align>, IComparable<Align>
{
    public static readonly Align ByteAligned = new();

    public static int AlignPadding(int value, int align)
    {
        Debug.Assert(align > 0);
        return (align - (value % align)) % align;
    }

    public static int AlignTo(int value, int align) => value + AlignPadding(value, align);

    public static Align ForBits(int bits) => ForBytes(AlignTo(bits, 8) / 8);
    public static Align ForBytes(int bytes) => AssumeAligned((int)BitOperations.RoundUpToPowerOf2((uint)bytes));
    public static Align AssumeAligned(int powerOfTwo) => powerOfTwo == 0 ? ByteAligned : new(powerOfTwo);
    //public static Align Of<T>() where T : struct => new(Marshal.SizeOf<T>());

    public static explicit operator Align(int powerOfTwo) => new(powerOfTwo);
    public static explicit operator int(Align align) => align.Value;

    public static bool operator ==(Align a, Align b) => a.Equals(b);
    public static bool operator !=(Align a, Align b) => !a.Equals(b);
    public static bool operator <=(Align a, Align b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Align a, Align b) => a.CompareTo(b) >= 0;
    public static bool operator <(Align a, Align b) => a.CompareTo(b) < 0;
    public static bool operator >(Align a, Align b) => a.CompareTo(b) > 0;

    private readonly int _shiftAmount;

    public int Value => 1 << _shiftAmount;

    public Align Previous
    {
        get
        {
            Debug.Assert(_shiftAmount != 0);
            return new(_shiftAmount - 1);
        }
    }

    public Align(int powerOfTwo)
    {
        Debug.Assert(powerOfTwo > 0, "Alignment must not be 0");
        Debug.Assert(BitOperations.IsPow2(powerOfTwo), "Alignment must be a power of 2");
        _shiftAmount = BitOperations.Log2((uint)powerOfTwo);
        Debug.Assert(_shiftAmount < 64);
    }

    public override int GetHashCode() => _shiftAmount.GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Align that && Equals(that);
    public bool Equals(Align that) => _shiftAmount == that._shiftAmount;
    public int CompareTo(Align that) => _shiftAmount.CompareTo(that._shiftAmount);
}

public readonly struct Size : IEquatable<Size>, IComparable<Size>
{
    public static Size FromBits(int bits) => new(bits);
    public static Size FromBytes(int bytes) => new(bytes * 8);

    public static bool operator ==(Size a, Size b) => a.Equals(b);
    public static bool operator !=(Size a, Size b) => !a.Equals(b);
    public static bool operator <=(Size a, Size b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Size a, Size b) => a.CompareTo(b) >= 0;
    public static bool operator <(Size a, Size b) => a.CompareTo(b) < 0;
    public static bool operator >(Size a, Size b) => a.CompareTo(b) > 0;

    public static Size operator +(Size lhs, Size rhs) => new(lhs._value + rhs._value);
    public static Size operator -(Size lhs, Size rhs) => new(lhs._value - rhs._value);
    public static Size operator *(Size lhs, int rhs) => new(lhs._value * rhs);
    public static Size operator *(int lhs, Size rhs) => new(rhs._value * lhs);

    private readonly int _value;

    public int Bits => _value;
    public int Bytes => Align.AlignTo(_value, 8) / 8;

    private Size(int value)
    {
        _value = value;
    }

    public Size AlignedTo(Align align) => FromBytes(Align.AlignTo(Bytes, align.Value));
    public Size AlignedTo(Size align) => FromBits(Align.AlignTo(Bits, align._value));

    public override int GetHashCode() => _value.GetHashCode();
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Size that && Equals(that);
    public bool Equals(Size that) => _value == that._value;
    public int CompareTo(Size that) => _value.CompareTo(that._value);
}

public static class FuccIRSigil
{
    public const string Global = "@";
    public const string Function = "@";
    public const string Struct = "$";
    public const string Parameter = "%";
    public const string Local = "%";
}

public abstract class FuccIRType(FuccIRTypeKind kind, FuccContext? context) : IEquatable<FuccIRType>
{
    private static int _idCounter = 0;

    public static bool operator ==(FuccIRType? a, FuccIRType? b) => a is null ? b is null : a.Equals(b);
    public static bool operator !=(FuccIRType? a, FuccIRType? b) => !(a == b);

    public int Id { get; } = Interlocked.Increment(ref _idCounter);

    public FuccIRTypeKind Kind { get; } = kind;
    public FuccContext? Context { get; } = context;

    public abstract Size Size { get; }
    public virtual Align Align => Align.ForBytes(Size.Bytes);

    public bool IsVoid => Kind == FuccIRTypeKind.Void;

    public override int GetHashCode() => Id;
    public override bool Equals(object? obj) => obj is FuccIRType that && Equals(that);
    public virtual bool Equals(FuccIRType? that) => that is not null && Id == that.Id;

    public sealed class Void : FuccIRType
    {
        public override Size Size { get; } = Size.FromBytes(1);
        public override Align Align { get; } = Align.ByteAligned;

        internal Void(FuccContext? context = null)
            : base(FuccIRTypeKind.Void, context)
        {
        }

        public override string ToString() => "void";
    }

    public sealed class Noreturn : FuccIRType
    {
        public override Size Size { get; } = Size.FromBytes(1);
        public override Align Align { get; } = Align.ByteAligned;

        internal Noreturn(FuccContext? context = null)
            : base(FuccIRTypeKind.Noreturn, context)
        {
        }

        public override string ToString() => "noreturn";
    }

    public sealed class Pointer : FuccIRType
    {
        public override Size Size { get; }
        public override Align Align { get; }

        internal Pointer(FuccContext context)
            : base(FuccIRTypeKind.Pointer, context)
        {
            Size = context.Target.SizeOfPointer;
            Align = context.Target.AlignOfPointer;
        }

        public override string ToString() => "pointer";
    }

    public sealed class Bool : FuccIRType
    {
        public override Size Size { get; } = Size.FromBytes(1);
        public override Align Align { get; } = Align.ByteAligned;

        internal Bool(FuccContext? context = null)
            : base(FuccIRTypeKind.Bool, context)
        {
        }

        public override string ToString() => "noreturn";
    }

    public sealed class Integer : FuccIRType
    {
        public override Size Size { get; }
        public override Align Align { get; }

        internal Integer(FuccContext? context, int sizeInBits)
            : base(FuccIRTypeKind.Integer, context)
        {
            Debug.Assert(sizeInBits > 0 && sizeInBits <= ushort.MaxValue, "Integer bit width must be in the range (0, 65535].");

            Size = Size.FromBits(sizeInBits);
            Align = Align.ForBits(sizeInBits);
        }

        public override string ToString() => $"int{Size.Bits}";
    }

    public sealed class Float : FuccIRType
    {
        public override Size Size { get; }
        public override Align Align { get; }

        internal Float(FuccContext? context, int sizeInBits)
            : base(FuccIRTypeKind.Float, context)
        {
            Debug.Assert(sizeInBits == 32 || sizeInBits == 64 || sizeInBits == 80 || sizeInBits == 128, "Integer bit width must be one of 32, 64, 80 or 128.");

            Size = Size.FromBits(sizeInBits);
            Align = Align.ForBits(sizeInBits);
        }

        public override string ToString() => $"float{Size.Bits}";
    }

    public sealed class Function : FuccIRType, IEquatable<Function>
    {
        public override Size Size { get; }
        public override Align Align { get; }

        public FuccIRType ReturnType { get; }
        public FuccIRType[] ParameterTypes { get; }

        internal Function(FuccContext context, FuccIRType returnType, FuccIRType[] parameterTypes)
            : base(FuccIRTypeKind.Function, context)
        {
            Size = context.Target.SizeOfPointer;
            Align = context.Target.AlignOfPointer;

            ReturnType = returnType;
            ParameterTypes = parameterTypes;
        }

        public override string ToString()
        {
            string parameterList = string.Join(", ", ParameterTypes.Select((type, i) => $"{type} {FuccIRSigil.Parameter}{i}"));
            string returnSpecifier = ReturnType.IsVoid ? "" : $" -> {ReturnType}";
            return $"func ({parameterList}){returnSpecifier}";
        }

        public override int GetHashCode() => Id;
        public override bool Equals(object? obj) => obj is Function that && Equals(that);
        public bool Equals(Function? that) => that is not null && ReturnType == that.ReturnType && ParameterTypes.SequenceEqual(that.ParameterTypes);
    }

    public sealed class Struct : FuccIRType
    {
        private readonly FuccIRType[] _fields;
        private readonly Size[] _fieldOffsets;

        public override Size Size { get; }
        public override Align Align { get; }

        public int FieldCount => _fields.Length;
        public FuccIRType this[int index] => _fields[index];

        internal Struct(FuccContext? context, FuccIRType[] fields)
            : base(FuccIRTypeKind.Struct, context)
        {
            _fields = fields;
            _fieldOffsets = fields.Length == 0 ? [] : new Size[fields.Length];

            if (fields.Length == 0)
            {
                Size = Size.FromBytes(1);
                Align = Align.ByteAligned;
            }
            else
            {
                var structSize = fields[0].Size;
                var structAlign = fields[0].Align;

                for (int i = 1; i < fields.Length; i++)
                {
                    Size fieldSize = fields[i].Size;
                    Align fieldAlign = fields[i].Align;

                    Size fieldOffset = structSize.AlignedTo(fieldAlign);
                    if (fieldAlign > structAlign)
                        structAlign = fieldAlign;

                    _fieldOffsets[i] = fieldOffset;
                    structSize = fieldOffset + fieldSize;
                }

                Size = structSize.AlignedTo(structAlign);
                Align = structAlign;
            }
        }

        public Size FieldOffsetAt(int index) => _fieldOffsets[index];

        public override string ToString()
        {
            string members = string.Join(", ", _fields.Select(f => f.ToString()));
            return $"struct {{ {members} }}";
        }
    }
}

public abstract class FuccIRValue(FuccContext context, FuccIRValueKind kind, SourceLocation location, FuccIRType type, string name)
{
    private static int _idCounter = 0;

    public int Id { get; } = Interlocked.Increment(ref _idCounter);

    internal bool IsExplicitlyNamed { get; set; }

    public FuccContext Context { get; } = context;
    public FuccIRValueKind Kind { get; } = kind;
    public SourceLocation Location { get; } = location;
    public FuccIRType Type { get; } = type;
    public string Name { get; internal set; } = name;

    public string Sigil { get; init; } = FuccIRSigil.Local;
    public string NameWithSigil => $"{Sigil}{Name}";

    public override string ToString() => $"{Type} {Sigil}{Name}";

    public class Constant : FuccIRValue
    {
        public FuccConstant Value { get; }

        internal Constant(FuccContext context, SourceLocation location, FuccIRType type, FuccConstant value)
            : base(context, FuccIRValueKind.Function, location, type, "")
        {
            Sigil = "";
            Value = value;
        }

        public override string ToString() => $"{Type} {Value}";
    }

    public class Global : FuccIRValue
    {
        internal Global(FuccContext context, SourceLocation location, FuccIRType type, string name)
            : base(context, FuccIRValueKind.Function, location, type, name)
        {
            Sigil = FuccIRSigil.Global;
        }
    }

    public class Function : FuccIRValue
    {
        public FuccIRType.Function FunctionType { get; }

        internal Function(FuccContext context, SourceLocation location, FuccIRType.Function functionType, string name)
            : base(context, FuccIRValueKind.Function, location, functionType, name)
        {
            Sigil = FuccIRSigil.Function;
            FunctionType = functionType;
        }

        internal Function(FuccContext context, SourceLocation location, FuccIRType returnType, FuccIRType[] parameterTypes, string name)
            : this(context, location, new FuccIRType.Function(context, returnType, parameterTypes), name)
        {
        }
    }
}

public abstract class FuccIRInstruction : FuccIRValue
{
    protected FuccIRInstruction(FuccContext context, FuccIRValueKind kind, SourceLocation location, FuccIRType type, string name)
        : base(context, kind, location, type, name)
    {
        Sigil = FuccIRSigil.Local;
    }
}
