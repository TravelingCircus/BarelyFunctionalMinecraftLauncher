namespace Common.Misc;

public class CompositeProgress
{
    public event Action<float> Changed; 
    public float Covered
    {
        get => _current;
        private set
        {
            if (_current + value > 1f) throw new Exception("Can't cover more that 100% of progress");
            _covered = Math.Clamp(value, 0f, 1f);
        }
    }
    private float _covered;
    public float Current
    {
        get => _current;
        private set => _current = Math.Clamp(value, 0f, 1f);
    }
    private float _current;
    private List<(ProgressTracker tracker, float share) > _trackers = new();

    public ProgressTracker AddTracker(float shareOfComposite)
    {
        ProgressTracker tracker = new ProgressTracker();
        _trackers.Add((tracker, shareOfComposite));
        UpdateCurrent();
        tracker.Changed += UpdateCurrent;
        return tracker;
    }

    private void UpdateCurrent()
    {
        Current = 0;
        foreach ((ProgressTracker tracker, float share) tuple in _trackers)
        {
            Current += tuple.tracker.Current * tuple.share;
        }
        Changed?.Invoke(Current);
    }
}