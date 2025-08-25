using System;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    [SerializeField] StatTypes name;
    [SerializeField] private int _value;
    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue;
    [SerializeField] private List<StatModifier> _statModifiers;

    public event Action OnChanged;
    public event Action OnMinValueReached;
    public event Action OnMaxValueReached;

    public Stat(StatTypes name, int minValue)
    {
        Name = name;
        _minValue = minValue;
    }

    public StatTypes Name { get { return name; } private set { name = value; } }
    public int Value
    {
        get { return _value; }
        set
        {
            if (value != _value)
            {
                OnChanged?.Invoke();
            }
            _value = Mathf.Clamp(value, _minValue, _maxValue);
            if (_value == _minValue)
            {
                OnMinValueReached?.Invoke();
                return;
            }
            if (_value == _maxValue)
            {
                OnMaxValueReached?.Invoke();
                return;
            }
        }
    }
    public int MinValue => _minValue;
    public int MaxValue => _maxValue;
    public List<StatModifier> StatModifiers => _statModifiers;

    public void ChangeMinValue(int newValue)
    {
        _minValue = newValue;

        if (_value <= _minValue)
            OnMinValueReached?.Invoke();
    }

    public void ChangeMaxValue(int newValue)
    {
        _maxValue = newValue;

        if (_value >= _maxValue)
            OnMaxValueReached?.Invoke();
    }

    public void AddStatModifier(string name, int value, int time)
    {
        var newStatModifier = new StatModifier(name, value, time);
        _statModifiers.Add(newStatModifier);
    }

    public void ClearModifiers()
    {
        _statModifiers.Clear();
    }
}

[System.Serializable]
public class StatModifier
{
    public string name;
    public int value;
    public int time;

    public StatModifier(string name, int value, int time)
    {
        this.name = name;
        this.value = value;
        this.time = time;
    }
}

public interface IStats
{
    public void InitializeStats();
}

public enum StatTypes
{
    Durability
}



