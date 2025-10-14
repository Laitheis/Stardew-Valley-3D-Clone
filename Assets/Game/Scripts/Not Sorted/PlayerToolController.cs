using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class PlayerToolController : MonoBehaviour
{
    [Serializable]
    class HandItemEntry
    {
        public ItemType type;
        public GameObject obj;
    }

    [Header("Raycast")]
    public LayerMask groundLayer;
    public float maxRayDistance = 100f;
    public float toolUseRadius = 5;

    [SerializeField] private List<HandItemEntry> handItemEntries;
    [SerializeField] private Animator _toolAnimator;
    [SerializeField] private ParticleSystem _waterParticles;
    [SerializeField] private Slider _cooldown;

    [Inject] private SelectedSlotController _selectedSlotHandler;
    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private HintVisualizer _hintVisual;
    [Inject] private CropController _cropManager;
    [Inject] private LootGeneratorHandler _lootGenerator;
    [Inject] private SignalBus _signalBus;
    [Inject(Id = "Player")] private GameObject _player;
    [Inject(Id = "ToolDelay")] private float _toolDelay;
    [Inject] private Canvas _mainCanvas;
    [Inject] private UIDragController _dragController;

    private ItemType activeTool = ItemType.None;
    private ItemDefinition activeItemDef;
    private Camera mainCam;
    private RaycastHit _raycastHit;
    private Vector3 _currTileWorld;
    private Vector3Int _currTileGrid;
    private bool _hitGround;
    private Dictionary<Vector3Int, TileState> _farmTiles;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    [SerializeField] private bool isToolUsageFrozen;

    private void Start()
    {
        raycaster = _mainCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        mainCam = Camera.main;
        _farmTiles = FarmManager.instance.farmTiles.TilesCollection;
    }

    private void Update()
    {
        HandleRaycast();

        HandleToolSwitchInput();
        HandleToolHint();
        HandleToolVisual();
    }

    public void OnClick()
    {
        TryUseTool();
    }

    public void OnEndClick()
    {
        isToolUsageFrozen = false;
    }

    void HandleRaycast()
    {
        // Update current mouse selected tile
        Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out _raycastHit, maxRayDistance, groundLayer))
        {
            _hitGround = true;
            _currTileWorld = TileContainer.TilePosFromWorld(_raycastHit.point);
            Vector3Int tile = new Vector3Int((int)_currTileWorld.x, (int)_currTileWorld.y, (int)_currTileWorld.z);
            Vector3Int swapped = new Vector3Int(tile.x, tile.z, tile.y);
            _currTileGrid = swapped;
        }
        else
        {
            _hitGround = false;
        }
    }

    private bool CheckRadius(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) < toolUseRadius;
    }

    private void HandleToolVisual()
    {
        ItemType handItem;
        if (_playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition == null)
            handItem = ItemType.None;
        else if (_playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition != null)
            handItem = _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition.type;
        else return;

        DisableOtherInstruments(handItem);

        HandItemEntry toActivate = handItemEntries.Find(e => e.type == handItem);

        if (toActivate != null)
        {
            toActivate.obj?.SetActive(true);
        }

        void DisableOtherInstruments(ItemType current)
        {
            foreach (var e in handItemEntries)
            {
                if (e.type != current)
                {
                    e.obj.SetActive(false);
                }
            }
        }
    }

    private void HandleToolSwitchInput()
    {
        var collection = _playerInv.Collection;

        if (collection[_selectedSlotHandler.SelectedSlotNum].ItemDefinition != null)
        {
            activeItemDef = collection[_selectedSlotHandler.SelectedSlotNum].ItemDefinition;
            activeTool = activeItemDef.type;
        }
        else
        {
            activeTool = ItemType.None;
        }
    }

    private void TryUseTool()
    {
        if (_hitGround)
        {
            Debug.Log("Try use tool");

            // Check - if we click on Inventory - return
            {
                pointerEventData = new PointerEventData(eventSystem)
                {
                    position = Input.mousePosition
                };

                var results = new List<RaycastResult>();
                raycaster.Raycast(pointerEventData, results);

                foreach (var element in results)
                {
                    if (element.gameObject.GetComponent<InventoryHandler>()) return;
                }
            }

            if (isToolUsageFrozen) return;
            if (_dragController.IsDragging)
            {
                isToolUsageFrozen = true;
                return;
            }
            if (_cooldown.value > 0) return;
            if (CheckRadius(_currTileWorld) == false) return;

            UseHand(_currTileGrid);

            // Start cooldown
            {
                _cooldown.gameObject.SetActive(true);
                _cooldown.value = 1;
                DOVirtual.Float(1, 0, _toolDelay, value =>
                {
                    _cooldown.value = value;
                    if (_cooldown.value == 0) _cooldown.gameObject.SetActive(false);
                });
            }

            switch (activeTool)
            {
                case ItemType.Hoe:
                    UseHoe(_currTileGrid);
                    break;
                case ItemType.WaterCan:
                    UseWater(_currTileGrid);
                    break;
                case ItemType.Scythe:
                    UseScythe();
                    break;
                case ItemType.Axe:
                    break;
                case ItemType.Pickaxe:
                    UsePickaxe();
                    break;
                case ItemType.Seed:
                    UseSeed(_currTileGrid);
                    break;
                case ItemType.Trash:
                    break;
                case ItemType.Material:
                    break;
                case ItemType.Crop:
                    break;
                case ItemType.Fertilize:
                    UseFertilize(_currTileGrid);
                    break;
                default:
                    break;
            }
        }
    }

    private void UseScythe()
    {
        BoxCollider damageZone = _player.GetComponent<PlayerController>().DamageZone;
        Collider[] hits = Physics.OverlapBox(damageZone.bounds.center, damageZone.bounds.extents, _player.transform.rotation);

        foreach (Collider hit in hits)
        {

        }
    }

    private void UsePickaxe()
    {
        if ((_farmTiles.TryGetValue(_currTileGrid, out TileState s) && s.objectOnTile is CropState))
        {
            _cropManager.UnplowTile(_currTileGrid);
        }
        if (s != null && s.objectOnTile is DebrisState debrisSt)
        {
            var destructibleObj = debrisSt.debrisVisualInstance.GetComponent<DestructibleObjectBase>();
            destructibleObj.TakeDamage(activeItemDef.Damage, ItemType.Pickaxe);
        }

        _toolAnimator.SetTrigger("Pickaxe");
    }

    private void UseHoe(Vector3Int tile)
    {
        _cropManager.PlowTile(tile);

        _toolAnimator.SetTrigger("Hoe");
    }

    private void UseWater(Vector3Int tile)
    {
        _cropManager.WaterTile(tile);

        _toolAnimator.SetTrigger("Water");

        _waterParticles.Play();
    }

    private void UseFertilize(Vector3Int tile)
    {
        FertilizeDefinition fertilize = _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition as FertilizeDefinition;
        if (fertilize != null)
        {
            if (_cropManager.IsPlowed(tile))
            {
                _playerInv[_selectedSlotHandler.SelectedSlotNum].SetCount(_playerInv[_selectedSlotHandler.SelectedSlotNum].Count - 1);
                string fertName = Enum.GetName(typeof(FertilizeType), fertilize.type);
                _cropManager.FertilizeTile(tile, fertName);
                Debug.Log("[ToolHandler] Fertilized " + tile);
            }
            else
            {
                Debug.Log("[ToolHandler] Can't fertilize this tile - tile is not plowed");
            }
        }
    }

    private void UseSeed(Vector3Int tile)
    {
        SeedDefinition seed = _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition as SeedDefinition;
        if (seed != null)
        {
            if (_cropManager.PlantSeed(tile, seed.cropModel))
            {
                _playerInv[_selectedSlotHandler.SelectedSlotNum].SetCount(_playerInv[_selectedSlotHandler.SelectedSlotNum].Count - 1);
            }
        }
    }

    private void UseHand(Vector3Int tile)
    {
        _cropManager.TryHarvestByHand(tile);
    }

    ItemType ToolSelected()
    {
        if (_playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition == null)
            return ItemType.None;
        else
            return _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition.type;
    }

    //true - green, false - red
    private void HandleToolHint()
    {
        ItemType tool = activeTool;

        switch (tool)
        {
            case ItemType.Seed:
                SeedHint();
                break;
            case ItemType.Trash:
                _hintVisual.Hide();
                break;
            case ItemType.Hoe:
                HoeHint();
                break;
            case ItemType.WaterCan:
                WaterHint();
                break;
            case ItemType.Scythe:
                //TODO
                break;
            case ItemType.Axe:
                _hintVisual.Hide();
                break;
            case ItemType.Pickaxe:
                PickaxeHint();
                break;
            case ItemType.None:
                _hintVisual.Hide();
                break;
            default:
                _hintVisual.Hide();
                break;

        }
    }

    private void PickaxeHint()
    {
        if (CheckRadius(_currTileWorld) == false)
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
            return;
        }
        if (!_farmTiles.TryGetValue(_currTileGrid, out TileState s))
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
            return;
        }
        if (s.objectOnTile != null)
            _hintVisual.ShowAvailable(_currTileWorld);
        else
            _hintVisual.ShowUnavailable(_currTileWorld);
    }

    void HoeHint()
    {
        if (CheckRadius(_currTileWorld) == false)
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
            return;
        }
        if ((_farmTiles.TryGetValue(_currTileGrid, out TileState s) && s.objectOnTile != null) || !(s != null && s.isFarm))
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
        }
        else
        {
            _hintVisual.ShowAvailable(_currTileWorld);
        }
    }

    void WaterHint()
    {
        if (CheckRadius(_currTileWorld) == false)
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
            return;
        }
        if (_cropManager.IsWatered(_currTileGrid) || !(_farmTiles.TryGetValue(_currTileGrid, out TileState s) && s.objectOnTile is CropState))
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
        }
        else
        {
            _hintVisual.ShowAvailable(_currTileWorld);
        }
    }

    void SeedHint()
    {
        if (CheckRadius(_currTileWorld) == false || _cropManager.CheckCropOnTile(_currTileGrid))
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
            return;
        }
        if (_cropManager.IsPlowed(_currTileGrid))
        {
            _hintVisual.ShowAvailable(_currTileWorld);
        }
        else
        {
            _hintVisual.ShowUnavailable(_currTileWorld);
        }
    }
}
