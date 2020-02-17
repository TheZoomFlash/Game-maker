using System;
using UnityEngine;

public class IntBar
{
    public int maxValue;
    public int CurrentValue { get; set; }
    public bool IsFull => CurrentValue.Equals(maxValue);
    public bool IsEmpty => CurrentValue.Equals(0);

    public IntBar(int m)
    {
        maxValue = CurrentValue = m;
    }

    public void SetValue(int v)
    {
        CurrentValue = v;
    }

    public void ChangeValue(int v)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + v, 0, maxValue);
    }

    public bool IsCanCast(int v)
    {
        return CurrentValue >= v;
    }
}
