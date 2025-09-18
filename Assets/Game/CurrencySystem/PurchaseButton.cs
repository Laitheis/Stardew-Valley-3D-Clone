using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    public UnityEvent onPurchaseSuccess;

    public string itemId;
    public int itemQuantity = 1;
    public int price;
    public CurrencyType currency;

    [SerializeField] TMP_Text _priceText;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(AttemptPurchase);

        if (_priceText != null)
        {
            _priceText.text = price.ToString();
        }
    }

    public void AttemptPurchase()
    {
        if (CurrencyManager.Instance.TryPurchase(itemId, currency, price, itemQuantity))
        {
            onPurchaseSuccess.Invoke();
        }
    }
}