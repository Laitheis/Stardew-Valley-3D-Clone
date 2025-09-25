using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class WorldObjectSelector : MonoBehaviour
{
    [Inject(Id = "OutlineGlow")] Material _highlightMaterial;

    private GameObject _currentTarget;
    private Material[] originalMaterials;

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
    }
}
