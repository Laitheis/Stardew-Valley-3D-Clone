using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public BoxCollider DamageZone;

    public float _speed = 5f;
    
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical; //+ transform.right * horizontal;
        Vector3 rotation = new Vector3(0, horizontal, 0);

        transform.position += direction * _speed * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(rotation);

        if (Input.GetKey(KeyCode.N))
        {
            Collider[] hits = Physics.OverlapBox(DamageZone.bounds.center, DamageZone.bounds.extents, transform.rotation);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue; // пропустить себя

                IDestructible hitObject = hit.GetComponent<IDestructible>();
                if (hitObject != null)
                {
                    hitObject.TakeDamage(10);
                }
            }
        }
    }
}

