using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class PurchaseButton : MonoBehaviour
{
    [HideInInspector] public new string name;
    [HideInInspector] public int itemQuantity = 1;
    [HideInInspector] public int price;
    [HideInInspector] public CurrencyType currency = CurrencyType.Gold;

    [Inject] private TraderHandler _traderHandler;

    private void Start()
    {
        var sceneContext = FindObjectOfType<SceneContext>();
        sceneContext.Container.Inject(this);

        Button button = GetComponent<Button>();
        button.onClick.AddListener(Try);
    }

    public void Try()
    {
        _traderHandler.TryPurchase(name, currency, price, itemQuantity);
    }

    private void OnValidate()
    {
        if (itemQuantity < 1)
        {
            itemQuantity = 1;
        }
    }
}