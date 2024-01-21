using System;

namespace BFML.WPF.Loading;

public class ProgressTracker : IProgress<float>
{
    public event Action Changed;
    public float Current
    {
        get => _current;
        private set => _current = Math.Clamp(value, 0f, 1f);
    }
    private float _current;

    public void Report(float value)
    {
        Current = value;
        Changed?.Invoke();
    }

    public void Add(float step)
    {
        Report(Current + step);
    }
}