namespace Utils;

public readonly struct Result<T> 
{
    public readonly T Value;
    public readonly Exception Error;
    private readonly bool _success;

    private Result(T v, Exception e, bool success)
    {
        Value = v;
        Error = e;
        _success = success;
    }

    public bool IsOk => _success;

    public static Result<T> Ok(T v) => new Result<T>(v, null, true);

    public static Result<T> Err(Exception e) => new Result<T>(default(T), e, false);
    
    public static implicit operator Result<T>(T v) => new Result<T>(v, null, true);
    
    public static implicit operator Result<T>(Exception e) => new Result<T>(default(T), e, false);

    public TR Match<TR>(Func<T, TR> success, Func<Exception, TR> failure) => _success ? success(Value) : failure(Error);

    public T Unwrap()
    {
        if (IsOk) return Value;
        throw Error;
    }
}

public readonly struct Result<T, TE> 
{
    public readonly T Value;
    public readonly TE Error;
    private readonly bool _success;

    private Result(T v, TE e, bool success)
    {
        Value = v;
        Error = e;
        _success = success;
    }

    public bool IsOk => _success;

    public static Result<T, TE> Ok(T v) => new Result<T, TE>(v, default(TE), true);

    public static Result<T, TE> Err(TE e) => new Result<T, TE>(default(T), e, false);
    
    public static implicit operator Result<T, TE>(T v) => new Result<T, TE>(v, default(TE), true);
    
    public static implicit operator Result<T, TE>(TE e) => new Result<T, TE>(default(T), e, false);

    public TR Match<TR>(Func<T, TR> success, Func<TE, TR> failure) => _success ? success(Value) : failure(Error);

    public void Match(Action<T> success, Action<TE> failure)
    {
        if (_success) success(Value);
        else failure(Error);
    }
}