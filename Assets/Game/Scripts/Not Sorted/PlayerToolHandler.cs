using InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerToolHandler : MonoBehaviour
{
    [Serializable]
    class HandItemEntry
    {
        public ItemType type;
        public GameObject obj;
    }
    [SerializeField] private List<HandItemEntry> _itemEntries;
    [SerializeField] private List<HandItemEntry> handItemEntries;
    [SerializeField] private Grid _grid;
    [SerializeField] private Animator _toolAnimator;
    [SerializeField] private ParticleSystem _waterParticles;

    public ItemType activeTool = ItemType.None;

    [Header("Planting")]
    public CropState selectedSeedModel;

    [Header("Raycast")]
    public LayerMask groundLayer;
    public float maxRayDistance = 100f;
    public float toolUseRadius = 5;

    private Camera mainCam;

    [Inject] private SelectedSlotHandler _selectedSlotHandler;
    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private HintVisualizer _hintVisual;
    [Inject] private CropHandler _cropManager;

    private RaycastHit _raycastHit;
    private Vector3Int _currTile;
    private bool _hitGround;
    private void Start()
    {
        mainCam = Camera.main;
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
            _currTile = CropHandler.TilePosFromWorld(_raycastHit.point);
        }
        else
        {
            _hitGround = false;
        }
    }
    bool CheckRadius(Vector3 pos)
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
            if (CheckRadius(_currTile) == false)
            {
                return;
            }
            switch (activeTool)
            {
                case ItemType.Hoe:
                    UseHoe(_currTile);

                    UseHand(_currTile);
                    break;
                case ItemType.WaterCan:
                    UseWater(_currTile);

                    UseHand(_currTile);
                    break;
                case ItemType.Scythe:

                    UseHand(_currTile);
                    break;
                case ItemType.Axe:

                    UseHand(_currTile);
                    break;
                case ItemType.Pickaxe:

                    UseHand(_currTile);
                    break;
                case ItemType.Seed:
                    UseSeed();
                    break;
                case ItemType.Regular:
                    break;
                case ItemType.Material:
                    break;
                case ItemType.Crop:
                    break;
                case ItemType.Fertilize:
                    UseFertilize(_currTile);
                    UseHand(_currTile);
                    break;
                default:
                    UseHand(_currTile);
                    break;
            }
        }
    }

    private void UseHoe(Vector3Int tile)
    {
        _cropManager.PlowTile(tile);

        _toolAnimator.SetTrigger("Hoe");
    }

    //private void UsePlant(Vector3 tile)
    //{
    //    if (selectedSeedModel == null) { Debug.Log("No seed selected"); return; }
    //    bool ok = CropManager.Instance.PlantSeed(tile, selectedSeedModel);
    //    if (!ok) { Debug.Log("Can't plant here"); return; }
    //    else
    //    {
    //        // TODO: отнимать семена из инвентаря
    //    }
    //}

    private void UseWater(Vector3Int tile)
    {
        _cropManager.WaterTile(tile);

        _toolAnimator.SetTrigger("Water");

        _waterParticles.Play();
    }

    //private void UseHarvest(Vector3Int tile)
    //{
    //    bool success = CropManager.Instance.HarvestTile(tile, out int quantity, out int quality);
    //    if (success)
    //    {
    //        Debug.Log($"Harvested {quantity} (quality {quality}) at {tile}");
    //        // TODO: добавить в инвентарь
    //    }
    //    else
    //    {
    //        Debug.Log("Nothing to harvest");
    //    }
    //}

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

    private void UseSeed()
    {
        SeedDefinition seed = _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition as SeedDefinition;
        if (seed != null)
        {
            if (_cropManager.PlantSeed(_currTile, seed.cropModel))
            {
                _playerInv[_selectedSlotHandler.SelectedSlotNum].SetCount(_playerInv[_selectedSlotHandler.SelectedSlotNum].Count - 1);
            }
        }
    }

    private void UseHand(Vector3 tile)
    {
        _cropManager.TryHarvestByHand(_currTile);
    }

    ItemType ToolSelected()
    {
        if (_playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition == null)
            return ItemType.None;
        else
            return _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition.type;
    }

    //true - зеленый, false - краный
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
        if (CheckRadius(_currTile) == false)
        {
            _hintVisual.ShowUnavailable(_currTile);
            return;
        }
        if (_cropManager.IsPlowed(_currTile))
        {
            _hintVisual.ShowUnavailable(_currTile);
        }
        else
        {
            _hintVisual.ShowAvailable(_currTile);
        }
    }
    void WaterHint()
    {
        if (CheckRadius(_currTile) == false)
        {
            _hintVisual.ShowUnavailable(_currTile);
            return;
        }
        if (_cropManager.IsWatered(_currTile) || !FarmManager.instance.farmTiles.TilesCollection.ContainsKey(_currTile))
        {
            _hintVisual.ShowUnavailable(_currTile);
        }
        else
        {
            _hintVisual.ShowAvailable(_currTile);
        }
    }
    void SeedHint()
    {
        if (CheckRadius(_currTile) == false || _cropManager.CheckCropOnTile(_currTile))
        {
            _hintVisual.ShowUnavailable(_currTile);
            return;
        }
        if (_cropManager.IsPlowed(_currTile))
        {
            _hintVisual.ShowAvailable(_currTile);
        }
        else
        {
            _hintVisual.ShowUnavailable(_currTile);
        }
    }

}
