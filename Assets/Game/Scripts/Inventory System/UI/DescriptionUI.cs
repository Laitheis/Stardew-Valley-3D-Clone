using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    public void Set(string desc)
    {
        _text.text = desc;
    }
    public void Display(bool value)
    {
        //_text.gameObject.SetActive(value);
        if (value == false)
        {
            _text.text = string.Empty;
        }
    }
}