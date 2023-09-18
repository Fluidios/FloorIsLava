using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObservableValue<T> where T : struct
{
    [SerializeField] private T _value;
    protected Action<T> _onValueChanged;

    [Networked]
    public T Value
    {
        get { return _value; }
        set
        {
            _value = value;
            _onValueChanged?.Invoke(value);
        }
    }

    public ObservableValue(T value)
    {
        _value = value;
    }


    public void Subscribe(Action<T> onValueChangedCallback)
    {
        _onValueChanged += onValueChangedCallback;
    }
    public void UnSubscribe(Action<T> onValueChangedCallback)
    {
        _onValueChanged -= onValueChangedCallback;
    }
}
