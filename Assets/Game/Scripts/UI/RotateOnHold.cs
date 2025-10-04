using UnityEngine;

public class RotateOnHold : MonoBehaviour
{
    public Transform target; // Объект, который будем вращать
    public float rotationSpeed = 100f; // Скорость вращения

    private bool isDragging = false;

    void Update()
    {
        // Проверяем удержание левой кнопки мыши
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
            //float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Вращаем объект по горизонтали и вертикали
            target.Rotate(Vector3.up, -horizontal, Space.World);
            //target.Rotate(Vector3.right, vertical, Space.World);
        }
    }
}
