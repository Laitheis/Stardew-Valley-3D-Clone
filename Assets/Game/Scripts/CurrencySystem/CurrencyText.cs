using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyText : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    [Inject] SignalBus _signalBus;

    private bool _isFistSet = true;

    void Start()
    {
        _signalBus.Subscribe<CurrencyEventArgs>(CurrencyChanged);
    }
    void CurrencyChanged(CurrencyEventArgs e)
    {
        _text.text = e.current.ToString();

        if (_isFistSet)
        {
            _isFistSet = false;
            return;
        }

        if (e.change == 0) return;

        string text = e.change < 0 ? e.change.ToString() : "+" + e.change.ToString();

        PopupTextSpawner.Instance.SpawnPopupText(text, transform.position);
    }

}
