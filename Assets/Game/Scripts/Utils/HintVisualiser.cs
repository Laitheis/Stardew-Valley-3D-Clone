using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

public class HintVisualizer : MonoBehaviour
{
    [SerializeField] private Color _availableColor;
    [SerializeField] private Color _unavailableColor;

    private GameObject _hint;

    private Material _shaderMaterial;

    private enum State { None, Available, Unavailable }
    private State _currentState = State.None;

    private CancellationTokenSource _cts;

    [Inject]
    public void Constructor([Inject(Id = "Available")] GameObject available)
    {
        _hint = Instantiate(available);
        _hint.SetActive(false);
        _shaderMaterial = _hint.transform.GetComponentInChildren<Renderer>().sharedMaterial;

    }

    public void ShowAvailable(Vector3 position)
    {
        ChangeState(State.Available);
        _hint.SetActive(true);
        _shaderMaterial.SetColor("_Color", _availableColor);
        _shaderMaterial.SetFloat("_SimpleVariant", 0f);
        _hint.transform.position = position;
    }

    public void ShowUnavailable(Vector3 position)
    {
        ChangeState(State.Unavailable);
        _hint.SetActive(true);
        _shaderMaterial.SetColor("_Color", _unavailableColor);
        _shaderMaterial.SetFloat("_SimpleVariant", 1f);
        _hint.transform.position = position;
    }

    public void Hide()
    {
        ChangeState(State.None);
        _hint.SetActive(false);
    }

    private void ChangeState(State newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        switch (_currentState)
        {
            case State.Available:
                EnableFade(true);
                break;
            case State.Unavailable:
                EnableFade(false);
                break;
            case State.None:
                // например выключаем оба
                EnableFade(false);
                break;
        }
    }

    private void EnableFade(bool available)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        Appearance(0f, 1.8f, 1.5f, _cts.Token).Forget();
    }

    private async UniTask Appearance(float from, float to, float duration, CancellationToken token)
    {
        float elapsed = 0;
        while(elapsed < duration)
        {
            token.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentValue = Mathf.Lerp(from, to, t);

            _shaderMaterial.SetFloat("_Power", currentValue);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        _shaderMaterial.SetFloat("_Power", to);
    }
}
