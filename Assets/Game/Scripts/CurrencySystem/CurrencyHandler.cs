using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class CurrencyHandler : MonoBehaviour
{
    public UnityEvent<CurrencyEventArgs> onCurrencyUpdated;
    public UnityEvent<CurrencyEventArgs> onCurrencyAdded;
    public UnityEvent<CurrencyEventArgs> onCurrencyDeducted;

    public bool negativeBalanceAllowed = false;

    public bool useCallbacks = false;

    public Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>();

    [SerializeField] private List<CurrencyEntry> initialCurrencies;

    [Inject] private SignalBus _signalBus;

    private void Start()
    {
        InitializeCurrencies();
    }
    void InitializeCurrencies()
    {
        foreach (CurrencyEntry entry in initialCurrencies)
        {
            currencies[entry.type] = entry.initialAmount;

            CurrencyEventArgs e = new CurrencyEventArgs() { type = entry.type, current = entry.initialAmount };
            if (useCallbacks)
            {
                onCurrencyUpdated?.Invoke(e);
            }
            
            _signalBus.Fire(e);
        }
    }

    public void AddCurrency(CurrencyType type, int amount, bool sendEvent = true)
    {
        if (!currencies.ContainsKey(type)) currencies[type] = 0;
        int c = currencies[type];
        c += amount;
        currencies[type] = c;

        CurrencyEventArgs e = new CurrencyEventArgs() { type = type, current = c };
        if (useCallbacks)
        {
            onCurrencyUpdated?.Invoke(e);
            onCurrencyAdded?.Invoke(e);
        }

        if (sendEvent)
        {
            _signalBus.Fire(e);
        }
    }

    public bool TryDeduct(CurrencyType type, int amount, bool sendEvent = true)
    {
        if (!currencies.ContainsKey(type))
        {
            return false;
        }
        if (amount > currencies[type])
        {
            if (negativeBalanceAllowed)
            {
                currencies[type] -= amount;
            }
            else
            {
                return false;
            }
        }

        currencies[type] -= amount;

        CurrencyEventArgs e = new CurrencyEventArgs() { type = type, current = currencies[type], change = amount };
        if (useCallbacks)
        {
            onCurrencyUpdated?.Invoke(e);
            onCurrencyDeducted?.Invoke(e);
        }
        if (sendEvent)
        {
            _signalBus.Fire(e);
        }
        return true;
    }

    public int GetCurrency(CurrencyType type)
    {
        return currencies.ContainsKey(type) ? currencies[type] : 0;
    }
}

[Serializable]
public class CurrencyEntry
{
    public CurrencyType type;
    public int initialAmount;
}

public enum CurrencyType
{
    Gold
}

public class CurrencyEventArgs
{
    public bool purchaseSuccess;

    public CurrencyType type;

    public int change;
    public int current;

    public string purchasedItem;
    public int purchasedCount;

    public object seller;
}