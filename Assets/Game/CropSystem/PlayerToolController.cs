using UnityEngine;

// Контроллер действий игрока (инструменты). Не трогаем перемещение/камеру.
// Требует: на игроке/камера есть Camera.main; курсор свободный — используем Raycast.
public class PlayerToolController : MonoBehaviour
{
    [SerializeField] Grid _grid;
    public enum Tool { None, Hoe, Plant, Water, Harvest, Fertilize }

    public Tool activeTool = Tool.None;

    [Header("Planting")]
    public CropModel selectedSeedModel; // выбранное семя в UI (drag'n'drop или inventory)

    [Header("Raycast")]
    public LayerMask groundLayer; // слой земли/тайлов
    public float maxRayDistance = 100f;

    private Camera mainCam;

    [SerializeField] GameObject _hintArrow;
    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        HandleHint();
        HandleToolSwitchInput();
        if (Input.GetMouseButtonDown(0))
        {
            TryUseTool();
        }
    }
    void HandleHint()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundLayer))
        {
            Vector3 gridPos = CropManager.TilePosFromWorld(hit.point);

            _hintArrow.transform.position = gridPos;
        }
    }
    private void HandleToolSwitchInput()
    {
        // Простейший переключатель (настраивай)
        if (Input.GetKeyDown(KeyCode.Alpha1)) activeTool = Tool.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) activeTool = Tool.Plant;
        if (Input.GetKeyDown(KeyCode.Alpha3)) activeTool = Tool.Water;
        if (Input.GetKeyDown(KeyCode.Alpha4)) activeTool = Tool.Harvest;
        if (Input.GetKeyDown(KeyCode.Alpha5)) activeTool = Tool.Fertilize;
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
                case Tool.Plant:
                    UsePlant(tile);
                    break;
                case Tool.Water:
                    UseWater(tile);
                    break;
                case Tool.Harvest:
                    UseHarvest(tile);
                    break;
                case Tool.Fertilize:
                    UseFertilize(tile);
                    break;
            }
        }
    }

    private void UseHoe(Vector3 tile)
    {
        // Вскопать: помечаем тайл как tilled и можно анимировать
        CropManager.Instance.TillTile(tile);
        // TODO: spawn small visual (в Inspector можно добавить prefab)
    }

    private void UsePlant(Vector3 tile)
    {
        if (selectedSeedModel == null) { Debug.Log("No seed selected"); return; }
        bool ok = CropManager.Instance.PlantSeed(tile, selectedSeedModel);
        if (!ok) Debug.Log("Can't plant here");
        else Debug.Log("Planted " + selectedSeedModel.displayName + " at " + tile);
        // TODO: отнимать семена из инвентаря
    }

    private void UseWater(Vector3 tile)
    {
        CropManager.Instance.WaterTile(tile);
        // воспроизводим анимацию полива у игрока — вне этого контроллера
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
