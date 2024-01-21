using System;

namespace BFML.WPF.Loading;

public class ProgressTracker
{
    public event Action<float> Changed;
    public string FormattedLabel { get; }
    public float Current { get; private set; }
    public TrackerState State { get; private set; }
    public string Label { get; private set; }
    public MeasurementUnits Units { get; private set; }
    public float MaxValue { get; private set; }

    public ProgressTracker(string label, float maxValue, MeasurementUnits units)
    {
        Label = label;
        MaxValue = maxValue;
        Units = units;
        State = TrackerState.Inactive;
        FormattedLabel = $"• {Label}";
    }

    public void Add(float step)
    {
        Set(Current + step);
    }

    public void Set(float value)
    {
        State = value >= MaxValue 
            ? TrackerState.Complete 
            : TrackerState.Running;
        
        Current = value;
        Current = Math.Clamp(Current, 0, MaxValue);
        
        Changed?.Invoke(Current / MaxValue);
    }
}