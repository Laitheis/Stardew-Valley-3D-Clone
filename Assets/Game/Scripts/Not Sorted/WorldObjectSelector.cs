using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class WorldObjectSelector : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)]
    private Color hlColor;

    [Inject(Id = "OutlineGlow")] private Material _highlightMaterial;
    [Inject(Id = "WorldTooltip")] private GameObject _worldTooltip;
    [Inject] private DefinitionDatabase _itemDatabase;
    [Inject] private CropManager _cropManager;
    [Inject] private FarmManager _farmManager;

    private GameObject _currentTarget;
    private WorldObjectType _currentType;
    private Material[] originalMaterials;
    private CancellationTokenSource hlCts;

    private void Start()
    {
        _worldTooltip = Instantiate(_worldTooltip);
        _worldTooltip.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && _currentTarget != null)
        {
            _worldTooltip.SetActive(true);
            _worldTooltip.GetComponent<Animator>().SetTrigger("Spawn");
            _worldTooltip.transform.position = GetTopPoint(_currentTarget.GetComponent<BoxCollider>(), _currentTarget.transform) + new Vector3(0, 0.4f, 0);
            _worldTooltip.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
            Invoke("FixForVerticalLayoutGroupFitting", 0.05f);

            WorldTooltipRefs refs;
            switch (_currentType)
            {
                case WorldObjectType.Item:
                    ItemDefinition item = _currentTarget.GetComponent<PickableItem>().Item.ItemDefinition;
                    refs = _worldTooltip.GetComponent<WorldTooltipRefs>();
                    refs.icon.sprite = item.Sprite;
                    refs.name.text = item.Name;
                    refs.description.text = item.Description;
                    break;
                case WorldObjectType.Crop:
                    refs = _worldTooltip.GetComponent<WorldTooltipRefs>();

                    Vector3Int tilePos = _currentTarget.GetComponent<TilePosHolder>().pos;
                    var cropState = _farmManager.farmTiles.TilesCollection[tilePos].objectOnTile as CropState;

                    CropModel cropModel = _itemDatabase.cropModels.Find(c => c.cropId == cropState.cropModelId);

                    refs.icon.sprite = cropModel.sprite;
                    refs.name.text = cropModel.displayName;
                    refs.description.text = "Stage: " + (cropState.currentStage + 1) + "/" + cropModel.daysPerStage.Length;

                    break;
                default:
                    break;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (!hitObj.GetComponent<WorldSelectable>())
            {
                ClearHighlight();
                return;
            }

            if (hitObj == _currentTarget) return;

            ClearHighlight();
            ApplyHighlight(hitObj);

            if (hitObj.TryGetComponent<WorldTooltipable>(out WorldTooltipable worldTooltipable))
            {
                _currentType = worldTooltipable.type;
            }
        }
        else
            ClearHighlight();
    }

    private void ApplyHighlight(GameObject obj)
    {
        //Renderer rend = obj.GetComponent<Renderer>();
        //if (rend == null) return;

        //// Save original materials
        //originalMaterials = rend.materials;

        //// Make copy and add highlight
        //List<Material> mats = new List<Material>(originalMaterials);
        //mats.Add(_highlightMaterial);

        //rend.materials = mats.ToArray();

        _currentTarget = obj;

        var outline = _currentTarget.GetComponent<Outline>();
        outline.OutlineColor = hlColor;
        outline.enabled = true;
    }

    private void ClearHighlight()
    {
        if (_currentTarget != null)
        {
            //Renderer rend = _currentTarget.GetComponent<Renderer>();
            //if (rend != null && originalMaterials != null)
            //{
            //    rend.materials = originalMaterials; // Return
            //}

            //originalMaterials = null;

            _currentTarget.GetComponent<Outline>().enabled = false;
            _currentTarget = null;
        }

        _worldTooltip.SetActive(false);
    }

    private Vector3 GetTopPoint(BoxCollider collider, Transform transform)
    {
        Vector3 center = collider.center;

        Vector3 topPoint = center + new Vector3(0, collider.size.y * 0.5f, 0);

        Vector3 worldTopPoint = transform.TransformPoint(topPoint);

        return worldTopPoint;
    }

    private void FixForVerticalLayoutGroupFitting()
    {
        _worldTooltip.GetComponentInChildren<VerticalLayoutGroup>().enabled = true;
    }
}
