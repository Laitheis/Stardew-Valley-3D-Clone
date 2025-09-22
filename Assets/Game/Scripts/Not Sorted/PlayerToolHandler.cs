using InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using Zenject;
using static UnityEditor.Progress;

// Контроллер действий игрока (инструменты). Не трогаем перемещение/камеру.
// Требует: на игроке/камера есть Camera.main; курсор свободный — используем Raycast.
public class PlayerToolHandler : MonoBehaviour
{
    [Serializable]
    class HandItemEntry
    {
        public ItemType type;
        public GameObject obj;
    }
    [SerializeField] List<HandItemEntry> _itemEntries;
    [SerializeField] private List<HandItemEntry> handItemEntries;
    
    [SerializeField] private Grid _grid;
    [SerializeField] private Animator _toolAnimator;

    [SerializeField] ParticleSystem _waterParticles;

    public ItemType activeTool = ItemType.None;

    [Header("Planting")]
    public CropModel selectedSeedModel; // выбранное семя в UI (drag'n'drop или inventory)

    [Header("Raycast")]
    public LayerMask groundLayer;
    public float maxRayDistance = 100f;
    public float toolUseRadius = 5;

    private Camera mainCam;

    [Inject] private SelectedSlotHandler _selectedSlotHandler;
    [Inject(Id = "PlayerInv")] private InventoryHandler _playerInv;
    [Inject] private HintVisualizer _hintVisual;

    RaycastHit _raycastHit;
    Vector3Int _currPtrTile;
    bool _hitGround;
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
        //Обновляем текущий выделенный мышкой тайл
        Ray r = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out _raycastHit, maxRayDistance, groundLayer))
        {
            _hitGround = true;
            _currPtrTile = CropManager.TilePosFromWorld(_raycastHit.point);
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
            if (CheckRadius(_currPtrTile) == false)
            {
                return;
            }
            switch (activeTool)
            {
                case ItemType.Hoe:
                    UseHoe(_currPtrTile);

                    UseHand(_currPtrTile);
                    break;
                case ItemType.WaterCan:
                    UseWater(_currPtrTile);

                    UseHand(_currPtrTile);
                    break;
                case ItemType.Scythe:

                    UseHand(_currPtrTile);
                    break;
                case ItemType.Axe:

                    UseHand(_currPtrTile);
                    break;
                case ItemType.Pickaxe:

                    UseHand(_currPtrTile);
                    break;
                case ItemType.Seed:
                    UseSeed();
                    break;
                default:
                    UseHand(_currPtrTile);
                    break;
            }
        }
    }

    private void UseHoe(Vector3 tile)
    {
        CropManager.Instance.PlowTile(tile);

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

    private void UseWater(Vector3 tile)
    {
        CropManager.Instance.WaterTile(tile);

        _toolAnimator.SetTrigger("Water");

        _waterParticles.Play();
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
    private void UseSeed()
    {
        SeedDefinition seed = _playerInv[_selectedSlotHandler.SelectedSlotNum].ItemDefinition as SeedDefinition;
        if (seed != null)
        {
            _playerInv[_selectedSlotHandler.SelectedSlotNum].SetCount(_playerInv[_selectedSlotHandler.SelectedSlotNum].Count-1);

            CropManager.Instance.PlantSeed(_currPtrTile, seed.cropModel);
        }
    }
    private void UseHand(Vector3 tile)
    {

        CropManager.Instance.TryHarvestByHand(_currPtrTile);
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
        if (CheckRadius(_currPtrTile) == false)
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
            return;
        }
        if (CropManager.Instance.IsPlowed(_currPtrTile))
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
        }
        else
        {
            _hintVisual.ShowAvailable(_currPtrTile);
        }
    }
    void WaterHint()
    {
        if (CheckRadius(_currPtrTile) == false)
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
            return;
        }
        if (CropManager.Instance.IsWatered(_currPtrTile) || !CropManager.Instance.tileToState.ContainsKey(_currPtrTile))
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
        }
        else
        {
            _hintVisual.ShowAvailable(_currPtrTile);
        }
    }
    void SeedHint()
    {
        if (CheckRadius(_currPtrTile) == false)
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
            return;
        }
        if (CropManager.Instance.IsPlowed(_currPtrTile))
        {
            _hintVisual.ShowAvailable(_currPtrTile);
        }
        else
        {
            _hintVisual.ShowUnavailable(_currPtrTile);
        }
    }
    
}
