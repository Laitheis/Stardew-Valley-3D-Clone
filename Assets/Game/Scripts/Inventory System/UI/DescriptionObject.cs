using InventorySystem;
using System.Collections;
using UnityEngine;

public class DescriptionObject : MonoBehaviour, IDescriptionOwner
{
    [TextArea]
    [SerializeField] string _description;

    
    public string GetDescription()
    {
        return _description;
    }
}
