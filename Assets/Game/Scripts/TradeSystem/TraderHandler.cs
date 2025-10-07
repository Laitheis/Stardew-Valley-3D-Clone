
using InventorySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TraderHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _tradeWindow;
    [SerializeField] private RectTransform _displayedItemsContainer;
    [SerializeField] private Image _displayedTraderIcon;
    [SerializeField] private TMPro.TextMeshProUGUI _displayedTradersText;
    [SerializeField] private GameObject _displayedItemPrefab;

    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private TradersTable _tradersTable;
    [Inject] private DefinitionDatabase _definitionDatabase;
    [Inject] private CurrencyHandler _currencyManager;
    [Inject] private SignalBus _signalBus;
    [Inject] private UIDragController _dragController;

    private string _currentTrader;

    public RectTransform TradeWindow { get => _tradeWindow; set => _tradeWindow = value; }

    public void SetCurrentTrader(string traderName)
    {
        _currentTrader = traderName;
        TraderModel trader = _tradersTable.FindByName(_currentTrader);
        _displayedTraderIcon.sprite = trader.Icon;
        _displayedTradersText.text = trader.TraderWelcomeText;
    }

    public void OpenTrade()
    {
        if (_currentTrader != null)
            FillTradeList();
    }

    private void FillTradeList()
    {
        TraderModel table = _tradersTable.FindByName(_currentTrader);

        List<TraderModel.Item> availableItems = new List<TraderModel.Item>();
        foreach (var item in table.Items)
        {
            if (item.seasons.Length == 0)
            {
                availableItems.Add(item);
                continue;
            }
            for (int i = 0; i < item.seasons.Length; i++)
            {
                if (item.seasons[i] == GameTimeService.instance.currentSeason)
                {
                    availableItems.Add(item);
                    break;
                }
            }
        }

        foreach (var item in availableItems)
        {
            GameObject displayedItem = Instantiate(_displayedItemPrefab, _displayedItemsContainer);
            TraderItemRefs refHolder = displayedItem.GetComponent<TraderItemRefs>();
            refHolder.Name.text = item.ItemDefinition.Name;
            refHolder.Icon.sprite = item.ItemDefinition.Sprite;
            refHolder.Price.text = item.ItemDefinition.Price.ToString();

            SetupPurchaseButton(displayedItem, item);
        }

    }
    void SetupPurchaseButton(GameObject go, TraderModel.Item item)
    {
        PurchaseButton btn = go.GetComponent<PurchaseButton>();
        btn.name = item.ItemDefinition.Name;
        btn.price = item.ItemDefinition.Price;
    }

    public bool TryPurchase(string itemName, CurrencyType currency, int pricePerPiece, int quantity, object seller = null)
    {
        if (_dragController.ItemInstance != null && _dragController.ItemInstance.IsFull())
        {
            return false;
        }

        var itemDef = _definitionDatabase.itemDefinitions.FirstOrDefault(i => i.name == itemName);
        if (_dragController.ItemInstance != null && _dragController.ItemInstance.ItemDefinition != itemDef)
        {
            return false;
        }

            int overallPrice = pricePerPiece * quantity;
        int before = _currencyManager.currencies[currency];

        bool success = _currencyManager.TryDeduct(currency, overallPrice, false);
        int after = _currencyManager.currencies[currency];

        var e = new CurrencyEventArgs
        {
            type = currency,
            current = after,
            purchaseSuccess = success,
            purchasedItem = itemName,
            purchasedCount = quantity,
            change = success ? after - before : 0
        };

        _signalBus.Fire(e);

        return success;
    }

    public void Sell(ItemInstance itemInstance)
    {
        int added = itemInstance.Price * itemInstance.Count;
        _currencyManager.currencies[CurrencyType.Gold] += added;

        var e = new CurrencyEventArgs
        {
            type = CurrencyType.Gold,
            current = _currencyManager.currencies[CurrencyType.Gold],
            purchaseSuccess = false,
            change = added
        };

        _signalBus.Fire(e);
    }

    public void Close()
    {
        for (int i = _displayedItemsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_displayedItemsContainer.GetChild(i).gameObject);
        }
    }
}

