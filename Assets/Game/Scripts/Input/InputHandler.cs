using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public interface IClickConsumer
{
    int Priority { get; }            // больший — выше приоритет
    bool OnClick();                  // true — клик обработан, дальше не передавать
    bool OnRightClick();             // при необходимости
    void OnEndClick();               // отпускание мыши
}

public class InputHandler : MonoBehaviour
{
    [Inject(Id = "Player")] private GameObject _player;
    [Inject] private UIDragController _dragController;

    private List<IClickConsumer> _consumers = new List<IClickConsumer>();
    private PlayerToolController _toolController;

    public void RegisterConsumer(IClickConsumer c)
    {
        if (!_consumers.Contains(c))
            _consumers.Add(c);
        // сортируем по приоритету (desc)
        _consumers = _consumers.OrderByDescending(x => x.Priority).ToList();
    }

    public void UnregisterConsumer(IClickConsumer c)
    {
        _consumers.Remove(c);
    }

    public void Init()
    {
        _toolController = _player.GetComponent<PlayerToolController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var c in _consumers)
            {
                if (c.OnClick())
                    break; // клик обработан, дальше не передаём
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
    }
}
