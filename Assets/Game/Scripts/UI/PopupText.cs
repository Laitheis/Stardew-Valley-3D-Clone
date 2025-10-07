using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
    public float moveUpSpeed = 50f;
    public float fadeDuration = 1f;

    private Text uiText;
    private Color originalColor;
    private float timer;

    private void Awake()
    {
        uiText = GetComponentInChildren<Text>();
        if (uiText != null) originalColor = uiText.color;
    }

    private void OnEnable()
    {
        timer = 0f;
        if (uiText != null) uiText.color = originalColor;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Сдвиг вверх
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Плавное исчезновение
        if (uiText != null)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, timer / fadeDuration);
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }

        // Уничтожаем после fadeDuration
        if (timer >= fadeDuration) Destroy(gameObject);
    }
}
