using System;

namespace BFML.WPF.Loading;

public class ProgressTracker
{
    public event Action Changed;
    public event Action Ended;
    public float Current { get; private set; }
    public TrackerState State { get; private set; }
    public string Label { get; private set; }
    private MeasurementUnits _units;
    private float _maxValue;

    public ProgressTracker(float maxValue, MeasurementUnits units, string label)
    {
        _maxValue = maxValue;
        _units = units;
        Label = label;
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