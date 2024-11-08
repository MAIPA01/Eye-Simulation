using System;

public class Lockable<T>
{
    private readonly object LockObj = new();
    private T Value;

    public Lockable(T value) {
        Value = value;
    }

    public void Lock(Action<T> action)
    {
        lock (LockObj)
        {
            action(Value);
        }
    }

    public void SetValue(T value)
    {
        lock (LockObj)
        {
            Value = value;
        }
    }

    public T GetValue()
    {
        return Value;
    }

    public static implicit operator T(Lockable<T> lockable) { return lockable.GetValue(); }
}