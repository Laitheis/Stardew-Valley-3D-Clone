using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextView : MonoBehaviour
{
    public static PopupTextView instance;

    [Header("Text Prefab")]
    public GameObject popupTextPrefab;

    [Header("Text Canvas")]
    public Canvas mainCanvas;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SpawnPopupText(string text, Vector3 worldPosition)
    {
        GameObject popupObj = Instantiate(popupTextPrefab, mainCanvas.transform);

        popupObj.transform.position = worldPosition;

        TMP_Text uiText = popupObj.GetComponentInChildren<TMP_Text>();
        if (uiText != null) uiText.text = text;
    }
}
