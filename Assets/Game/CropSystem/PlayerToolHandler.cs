using InventorySystem;
using UnityEditor.EditorTools;
using UnityEngine;
using Zenject;

// Контроллер действий игрока (инструменты). Не трогаем перемещение/камеру.
// Требует: на игроке/камера есть Camera.main; курсор свободный — используем Raycast.
public class PlayerToolHandler : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Animator _toolAnimator;

    public Tool activeTool = Tool.None;

    [Header("Planting")]
    public CropModel selectedSeedModel; // выбранное семя в UI (drag'n'drop или inventory)

    [Header("Raycast")]
    public LayerMask groundLayer; // слой земли/тайлов
    public float maxRayDistance = 100f;

    private Camera mainCam;

    [Inject] private SelectedSlotHandler _selectedSlotHandler;
    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject(Id = "Available")] private GameObject _available;
    [Inject(Id = "Unavailable")] private GameObject _unavailable;


    private void Start()
    {
        mainCam = Camera.main;
        _available = Instantiate(_available);
        _unavailable = Instantiate(_unavailable);
        _available.SetActive(false);
        _unavailable.SetActive(false);
    }

    private void Update()
    {
        HandleHintVisual();
        HandleToolSwitchInput();
        if (Input.GetMouseButtonDown(0))
        {
            TryUseTool();
        }
    }

    void HandleHintVisual()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundLayer))
        {
            Vector3 gridPos = CropManager.TilePosFromWorld(hit.point);

            if (activeTool == Tool.Hoe)
            {
                if (CropManager.Instance.IsPlowed(gridPos))
                {
                    _available.SetActive(false);
                    _unavailable.SetActive(true);
                    _unavailable.transform.position = gridPos;
                }
                else
                {
                    _unavailable.SetActive(false);
                    _available.SetActive(true);
                    _available.transform.position = gridPos;
                }
            }

            if (activeTool == Tool.Water)
            {
                if (CropManager.Instance.IsWatered(gridPos))
                {
                    _available.SetActive(false);
                    _unavailable.SetActive(true);
                    _unavailable.transform.position = gridPos;
                }
                else
                {
                    _unavailable.SetActive(false);
                    _available.SetActive(true);
                    _available.transform.position = gridPos;
                }
            }
        }

        if (activeTool != Tool.Water && activeTool != Tool.Hoe)
        {
            _available.SetActive(false);
            _unavailable.SetActive(false);
        }
    }

    private void HandleToolSwitchInput()
    {
        var collection = _playerInv.Collection;
        if ((collection[_selectedSlotHandler.SelectedSlotNum].ItemDefinition is ToolDefinition tool) == false)
        {
            activeTool = Tool.None;
            return;
        }

        activeTool = tool.ToolType;
    }

    private void TryUseTool()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundLayer))
        {
            Vector3 tile = CropManager.TilePosFromWorld(hit.point);

            switch (activeTool)
            {
                case Tool.Hoe:
                    UseHoe(tile);
                    break;
                case Tool.Water:
                    UseWater(tile);
                    break;
                case Tool.Harvest:
                    break;
                case Tool.Axe:
                    break;
                case Tool.Pickaxe:
                    break;
            }
        }
    }

    private void UseHoe(Vector3 tile)
    {
        CropManager.Instance.PlowTile(tile);

        _toolAnimator.SetTrigger("Hoe");
    }

    private void UsePlant(Vector3 tile)
    {
        if (selectedSeedModel == null) { Debug.Log("No seed selected"); return; }
        bool ok = CropManager.Instance.PlantSeed(tile, selectedSeedModel);
        if (!ok) { Debug.Log("Can't plant here"); return; }
        else
        {
            // TODO: отнимать семена из инвентаря
        }
    }

    private void UseWater(Vector3 tile)
    {
        CropManager.Instance.WaterTile(tile);
    }

    private void UseHarvest(Vector3 tile)
    {
        bool success = CropManager.Instance.HarvestTile(tile, out int quantity, out int quality);
        if (success)
        {
            Debug.Log($"Harvested {quantity} (quality {quality}) at {tile}");
            // TODO: добавить в инвентарь
        }
        else
        {
            Debug.Log("Nothing to harvest");
        }
    }

    private void UseFertilize(Vector3 tile)
    {
        CropManager.Instance.FertilizeTile(tile);
        Debug.Log("Fertilized " + tile);
        // TODO: отнять удобрение из инвентаря
    }
}
