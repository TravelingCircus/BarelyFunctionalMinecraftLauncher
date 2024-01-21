using System;

namespace BFML.WPF.Loading;

public class ProgressTracker
{
    public event Action Changed;
    public float Current { get; private set; }
    public TrackerState State { get; private set; }
    private MeasurementUnits _units;
    private float _maxValue;

    public ProgressTracker(float maxValue, MeasurementUnits units)
    {
        _maxValue = maxValue;
        _units = units;
    }

    public void Add(float step)
    {
        Set(Current + step);
    }

    public void Set(float value)
    {
        Current = value;
        Changed?.Invoke();
    }
}