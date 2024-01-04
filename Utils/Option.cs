namespace Utils;

public readonly struct Option<T>
{
    public static Option<T> None => default;
    public static Option<T> Some(T value) => new Option<T>(value);

    public readonly bool IsSome;
    public readonly T Value;

    private Option(T value)
    {
        Value = value;
        IsSome = Value is { };
    }

    public bool IfSome(out T value)
    {
        value = Value;
        return IsSome;
    }
}
