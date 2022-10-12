namespace Common.Misc;

public class ProgressTracker : IProgress<float>
{
    public event Action Changed;
    public float Current
    {
        get => _current;
        private set => Math.Clamp(value, 0f, 1f);
    }
    private float _current;

    public void Report(float value)
    {
        Changed?.Invoke();
    }

    public void Add(float step)
    {
        Current += step;
        Report(Current);
    }
}