using UnityEngine;

[RequireComponent(typeof(Outline))]
public class WorldSelectable : MonoBehaviour
{
    private void Reset()
    {
        var outline = GetComponent<Outline>();
        if(outline)
        {
            outline.enabled = false;
        }
    }
}



