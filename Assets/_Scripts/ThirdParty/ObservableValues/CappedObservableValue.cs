using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CappedObservableValue<T> : ObservableValue<T> where T : struct, IComparable<T>
{
    [SerializeField] private T _maxValue;
    public T MaxTValue
    {
        get
        {
            return _maxValue;
        }
    }

    public CappedObservableValue(T maxValue) : base(maxValue)
    {
        _maxValue = maxValue;
    }

    public bool MaxValueReached
    {
        get { return Value.CompareTo(MaxTValue) >= 0; }
    }
    public void SetNewMaxValue(T newMaxValue)
    {
        _maxValue = newMaxValue;
        if (_onValueChanged != null) //listeners more than zero
            _onValueChanged(Value);
    }
    public void Reset()
    {
        Value = _maxValue;
    }
}

