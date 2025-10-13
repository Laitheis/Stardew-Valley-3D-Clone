using UnityEngine;
using Zenject;


public class InputHandler : MonoBehaviour
{
    [Inject(Id = "Player")] private GameObject _player;
    [Inject] private UIDragController _dragController;

    private PlayerToolController _toolController;

    public void Init()
    {
        _toolController = _player.GetComponent<PlayerToolController>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _toolController.OnClick();
            _dragController.OnClick();
        }
        if (Input.GetMouseButton(1))
        {
            _dragController.OnClick();
        }
        if(Input.GetMouseButtonUp(0))
        {
            _toolController.OnEndClick();
        }
    }
}
