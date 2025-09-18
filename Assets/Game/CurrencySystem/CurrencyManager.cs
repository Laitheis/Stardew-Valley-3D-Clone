using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

public class CurrencyEventArgs
{
    public bool purchaseSuccess;

    public CurrencyType type;
    /// <summary>
    /// Negative on deduct
    /// </summary>
    public int added;
    public int current;

    public string purchasedItem;
    public int purchasedCount;

    public object seller;
}
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] List<CurrencyEntry> initialCurrencies;

    Dictionary<CurrencyType, int> _currencies = new Dictionary<CurrencyType, int>();

    // Callbacks
    public UnityEvent<CurrencyEventArgs> onCurrencyUpdated;
    public UnityEvent<CurrencyEventArgs> onCurrencyAdded;
    public UnityEvent<CurrencyEventArgs> onCurrencyDeducted;

    public UnityEvent<CurrencyEventArgs> onPurchase;
    public UnityEvent<CurrencyEventArgs> onSell;

    public bool negativeBalanceAllowed = false;

    public bool useEventBus = true;
    public bool useCallbacks = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeCurrencies();
    }
    void InitializeCurrencies()
    {
        foreach (CurrencyEntry entry in initialCurrencies)
        {
            _currencies[entry.type] = entry.initialAmount;

            CurrencyEventArgs e = new CurrencyEventArgs() { type = entry.type, current = entry.initialAmount };
            if (useCallbacks)
            {
                onCurrencyUpdated?.Invoke(e);
            }
            if (useEventBus)
            {
                EventBus.Publish(e);
            }
        }
    }

    public void AddCurrency(CurrencyType type, int amount)
    {
        if (!_currencies.ContainsKey(type)) _currencies[type] = 0;
        int c = _currencies[type];
        c += amount;
        _currencies[type] = c;

        CurrencyEventArgs e = new CurrencyEventArgs() { type = type, current = c };
        if (useCallbacks)
        {
            onCurrencyUpdated?.Invoke(e);
            onCurrencyAdded?.Invoke(e);
        }
        if (useEventBus)
        {
            EventBus.Publish(e);
        }
    }

    public bool TryDeduct(CurrencyType type, int amount)
    {
        if (!_currencies.ContainsKey(type))
        {
            return false;
        }
        if (amount > _currencies[type])
        {
            if (negativeBalanceAllowed)
            {
                _currencies[type] -= amount;
            }
            else
            {
                return false;
            }
        }

        _currencies[type] -= amount;

        CurrencyEventArgs e = new CurrencyEventArgs() { type = type, current = _currencies[type], added = amount };
        if (useCallbacks)
        {
            onCurrencyUpdated?.Invoke(e);
            onCurrencyDeducted?.Invoke(e);
        }
        if (useEventBus)
        {
            EventBus.Publish(e);
        }
        return true;
    }

    public int GetCurrency(CurrencyType type)
    {
        return _currencies.ContainsKey(type) ? _currencies[type] : 0;
    }

    public bool TryPurchase(string item, CurrencyType currency, int pricePerPiece, int quantity, object seller = null)
    {
        bool result;
        int currencyBeforeOperation = _currencies[currency]; 
        int overallPrice = pricePerPiece * quantity;

        CurrencyEventArgs e = new CurrencyEventArgs();
        if (TryDeduct(currency ,overallPrice))
        {
            int currencyAfterOperation = _currencies[currency];

            e.type = currency;
            e.current = currencyAfterOperation;
            e.purchaseSuccess = true;
            e.added = currencyBeforeOperation - currencyAfterOperation;
            e.seller = seller;

            result = true;
        }
        else
        {
            e.type = currency;
            e.current = _currencies[currency];
            e.purchaseSuccess = false;
            e.seller = seller;

            result = false;
        }

        EventBus.Publish<CurrencyEventArgs>(e);

        return result;
    }
    public void Sell()
    {

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
    Gold,
    Perls,
    Rubies,
    Saphires,
    Diamonds
}