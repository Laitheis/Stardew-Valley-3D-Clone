using UnityEngine;

public class RotateOnHold : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 100f;

    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && target != null)
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            target.Rotate(Vector3.up, -horizontal, Space.World);
            //target.Rotate(Vector3.right, vertical, Space.World);
        }
    }
}
