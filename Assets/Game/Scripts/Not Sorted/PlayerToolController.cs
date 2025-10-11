using InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerToolController : MonoBehaviour
{
    [Serializable]
    class HandItemEntry
    {
        public ItemType type;
        public GameObject obj;
    }
    [SerializeField] private List<HandItemEntry> _itemEntries;
    [SerializeField] private List<HandItemEntry> handItemEntries;
    [SerializeField] private Animator _toolAnimator;
    [SerializeField] private ParticleSystem _waterParticles;

    public ItemType activeTool = ItemType.None;

    [Header("Raycast")]
    public LayerMask groundLayer;
    public float maxRayDistance = 100f;
    public float toolUseRadius = 5;

    private Camera mainCam;

    [Inject] private SelectedSlotController _selectedSlotHandler;
    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private HintVisualizer _hintVisual;
    [Inject] private CropController _cropManager;

    private RaycastHit _raycastHit;
    private Vector3 _currTileWorld;
    private Vector3Int _currTileGrid;
    private bool _hitGround;
    private Dictionary<Vector3Int, TileState> _farmTiles;

    private void Start()
    {
        mainCam = Camera.main;
        _farmTiles = FarmManager.instance.farmTiles.TilesCollection;
    }

    private void Update()
    {
        HandleRaycast();

        HandleToolSwitchInput();
        HandleToolHint();
        HandleToolVisual();
        if (Input.GetMouseButtonDown(0))
        {
            TryUseTool();
        }
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
            activeTool = collection[_selectedSlotHandler.SelectedSlotNum].ItemDefinition.type;
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
            if (CheckRadius(_currTileWorld) == false)
            {
                return;
            }
            switch (activeTool)
            {
                case ItemType.Hoe:
                    UseHoe(_currTileGrid);

                    UseHand(_currTileGrid);
                    break;
                case ItemType.WaterCan:
                    UseWater(_currTileGrid);

                    UseHand(_currTileGrid);
                    break;
                case ItemType.Scythe:

                    UseHand(_currTileGrid);
                    break;
                case ItemType.Axe:

                    UseHand(_currTileGrid);
                    break;
                case ItemType.Pickaxe:

                    UseHand(_currTileGrid);
                    break;
                case ItemType.Seed:
                    UseSeed(_currTileGrid);
                    break;
                case ItemType.Regular:
                    break;
                case ItemType.Material:
                    break;
                case ItemType.Crop:
                    break;
                case ItemType.Fertilize:
                    UseFertilize(_currTileGrid);
                    UseHand(_currTileGrid);
                    break;
                default:
                    UseHand(_currTileGrid);
                    break;
            }
        }
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
        _cropManager.TryHarvestByHand(_currTileGrid);
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
            case ItemType.Regular:
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
                break;
            case ItemType.None:
                _hintVisual.Hide();
                break;
            default:
                _hintVisual.Hide();
                break;

        }
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
