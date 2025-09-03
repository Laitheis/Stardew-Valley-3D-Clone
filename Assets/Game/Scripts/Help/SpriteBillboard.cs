using Unity.VisualScripting;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.left);
    }
}

