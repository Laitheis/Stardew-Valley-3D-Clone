using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextSpawner : MonoBehaviour
{
    public static PopupTextSpawner Instance;

    [Header("Prefab текста")]
    public GameObject popupTextPrefab;

    [Header("Canvas для текста")]
    public Canvas mainCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Спавнит всплывающий текст в указанной позиции экрана
    /// </summary>
    public void SpawnPopupText(string text, Vector3 worldPosition)
    {
        // Создаём объект под канвасом
        GameObject popupObj = Instantiate(popupTextPrefab, mainCanvas.transform);

        popupObj.transform.position = worldPosition;

        // Устанавливаем текст
        TMP_Text uiText = popupObj.GetComponentInChildren<TMP_Text>();
        if (uiText != null) uiText.text = text;
    }
}
