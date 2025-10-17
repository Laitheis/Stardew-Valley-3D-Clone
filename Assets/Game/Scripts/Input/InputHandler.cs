using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public interface IClickConsumer
{
    public int ClickPriority { get; }
    public bool OnClick();
    public bool OnRightClick();
    public void OnEndClick();
    public bool OnHold();
}

public class InputHandler : MonoBehaviour
{
    [Inject(Id = "Player")] private GameObject _player;
    [Inject] private UIDragController _dragController;

    private List<IClickConsumer> _consumers = new List<IClickConsumer>();
    private PlayerToolController _toolController;

    public void Init()
    {
        _toolController = _player.GetComponent<PlayerToolController>();
    }

    public void RegisterConsumer(IClickConsumer c)
    {
        if (!_consumers.Contains(c))
            _consumers.Add(c);
        _consumers = _consumers.OrderByDescending(x => x.ClickPriority).ToList();
    }

    public void UnregisterConsumer(IClickConsumer c)
    {
        _consumers.Remove(c);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var c in _consumers)
            {
                if (c.OnClick())
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var c in _consumers)
                c.OnEndClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (var c in _consumers)
            {
                if (c.OnRightClick())
                    break;
            }
        }

        if(Input.GetMouseButton(0))
        {
            foreach (var c in _consumers)
            {
                if (c.OnHold())
                    break;
            }
        }
    }
}
