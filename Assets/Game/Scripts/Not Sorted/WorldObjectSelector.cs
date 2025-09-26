using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class WorldObjectSelector : MonoBehaviour
{
    [Inject(Id = "OutlineGlow")] private Material _highlightMaterial;
    [Inject(Id = "WorldTooltip")] private GameObject _worldTooltip;
    [Inject] private ItemDatabase _itemDatabase;

    private GameObject _currentTarget;
    private Material[] originalMaterials;

    private void Start()
    {
        _worldTooltip = Instantiate(_worldTooltip);
        _worldTooltip.SetActive(false);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("Selectable"))
            {
                if (hitObj != _currentTarget)
                {
                    ClearHighlight();
                    ApplyHighlight(hitObj);

                    if (hitObj.TryGetComponent<WorldTooltipable>(out WorldTooltipable worldTooltipable))
                    {
                        _worldTooltip.SetActive(true);
                        _worldTooltip.transform.position = hitObj.transform.position;
                        _worldTooltip.GetComponent<Animator>().SetTrigger("Spawn");

                        WorldTooltipRefs refs;
                        switch (worldTooltipable.type)
                        {
                            case WorldObjectType.Item:
                                ItemDefinition item = hitObj.GetComponent<PickableItem>().Item.ItemDefinition;
                                refs = _worldTooltip.GetComponent<WorldTooltipRefs>();
                                refs.icon.sprite = item.Sprite;
                                refs.name.text = item.Name;
                                refs.description.text = item.Description;
                                break;
                            case WorldObjectType.Crop:
                                refs = _worldTooltip.GetComponent<WorldTooltipRefs>();

                                Vector3Int tilePos = hitObj.GetComponent<TilePosHolder>().pos;
                                var tileState = CropManager.Instance.tileToState[tilePos];

                                CropModel cropModel = _itemDatabase.cropModels.Find(c => c.cropId == tileState.cropModelId);

                                refs.icon.sprite = cropModel.sprite;
                                refs.name.text = cropModel.displayName;
                                refs.description.text = "Stage: "  + tileState.currentStage + "/" + cropModel.daysPerStage.Length;

                                break;
                            default:
                                break;
                        }
                    }
                }
                return;
            }
        }

        ClearHighlight();
    }

    void ApplyHighlight(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend == null) return;

        // Save original materials
        originalMaterials = rend.materials;

        // Make copy and add highlight
        List<Material> mats = new List<Material>(originalMaterials);
        mats.Add(_highlightMaterial);

        rend.materials = mats.ToArray();

        _currentTarget = obj;
    }

    void ClearHighlight()
    {
        if (_currentTarget != null)
        {
            Renderer rend = _currentTarget.GetComponent<Renderer>();
            if (rend != null && originalMaterials != null)
            {
                rend.materials = originalMaterials; // Return
            }

            _currentTarget = null;
            originalMaterials = null;
        }

        _worldTooltip.SetActive(false);
    }
}
