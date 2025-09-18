
using InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

public class TradingHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _tradeWindow;
    [SerializeField] private RectTransform _displayedItemsContainer;
    [SerializeField] private Image _displayedTraderIcon;
    [SerializeField] private TMPro.TextMeshProUGUI _displayedTradersText;
    [SerializeField] private GameObject _displayedItemPrefab;

    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private TradersTable _tradersTable;

    private string _currentTrader;

    public void SetCurrentTrader(string traderName)
    {
        _currentTrader = traderName;
        TraderModel trader = _tradersTable.FindByName(_currentTrader);
        _displayedTraderIcon.sprite = trader.Icon;
        _displayedTradersText.text = trader.TraderWelcomeText;
    }

    public void OpenTrade()
    {
        _tradeWindow.gameObject.SetActive(true);

        if (_currentTrader != null)
            FillTradeList();
    }

    private void FillTradeList()
    {
        TraderModel table = _tradersTable.FindByName(_currentTrader);

        List<TraderModel.Item> availableItems = new List<TraderModel.Item>();
        foreach (var item in table.Items)
        {
            if(item.seasons.Length == 0)
            {
                availableItems.Add(item);
                continue;
            }
            for (int i = 0; i < item.seasons.Length; i++)
            {
                if (item.seasons[i] == GameTimeManager.Instance.currentSeason)
                {
                    availableItems.Add(item);
                    break;
                }
            }
        }

        foreach (var item in availableItems)
        {
            GameObject displayedItem = Instantiate(_displayedItemPrefab, _displayedItemsContainer);
            DisplayedTraderItemHolder refHolder = displayedItem.GetComponent<DisplayedTraderItemHolder>();
            refHolder.Name.text = item.ItemDefinition.Name;
            refHolder.Icon.sprite = item.ItemDefinition.Sprite;
            refHolder.Price.text = item.ItemDefinition.Price.ToString();
        }

    }
}

