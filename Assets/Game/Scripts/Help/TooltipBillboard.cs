using Unity.VisualScripting;
using UnityEngine;

public class TooltipBillboard : MonoBehaviour
{
    private Vector3 _rotation;

    private void Update()
    {
        Vector3 dir = Camera.main.transform.position - transform.position;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        _rotation = lookRot.eulerAngles;

        _rotation = new Vector3(0, _rotation.y + 180, 0);

        transform.rotation = Quaternion.Euler(_rotation);
    }
}

