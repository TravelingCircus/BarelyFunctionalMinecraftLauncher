namespace CommonData.Misc;

public class CompositeProgress
{
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
        private set => Math.Clamp(value, 0f, 1f);
    }
    private float _current;
    private List<(ProgressTracker tracker, float share)> _trackers = new();

    public void AddTracker(ProgressTracker tracker, float shareOfComposite)
    {
        _trackers.Add((tracker, shareOfComposite));
        UpdateCurrent();
    }

    private void UpdateCurrent()
    {
        
    }
}