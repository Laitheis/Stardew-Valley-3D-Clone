using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [Inject] private MainGameManager _farmManager;
    [Inject] private DebrisGeneratorController _debrisGen;
    [Inject] private InputHandler _inputHandler;
    [Inject] private GameTimeHandler _timeHandler;
    private void Awake()
    {
        _timeHandler.Init();
        _debrisGen.Init();
        _farmManager.Init();
        _inputHandler.Init();
    }
}


