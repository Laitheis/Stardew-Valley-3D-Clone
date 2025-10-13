using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [Inject] private CropController _cropHandler;
    [Inject] private FarmManager _farmManager;
    [Inject] private DebrisGeneratorController _debrisGen;
    [Inject] private InputHandler _inputHandler;
    private void Awake()
    {
        _debrisGen.Init();
        _farmManager.Init();
        _cropHandler.InitTiles();
        _inputHandler.Init();
    }
}


