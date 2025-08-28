using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    public BoxCollider DamageZone;

    [Min(0)] public float _speed = 5f;
    [Min(0)] public float delay = 1f;

    private bool _isAssignToAttack = true;
    
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical;
        Vector3 rotation = new Vector3(0, horizontal, 0);

        transform.position += direction * _speed * Time.deltaTime;
        transform.rotation = transform.rotation * Quaternion.Euler(rotation);

        if (Input.GetKey(KeyCode.N))
        {
            if (!_isAssignToAttack) return;

            Collider[] hits = Physics.OverlapBox(DamageZone.bounds.center, DamageZone.bounds.extents, transform.rotation);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IDestructible hitObject = hit.GetComponent<IDestructible>();
                if (hitObject != null)
                {
                    hitObject.TakeDamage(10);
                }
            }

            WaitSeconds(delay).Forget();
        }
    }

    private async UniTask WaitSeconds(float seconds)
    {
        _isAssignToAttack = false;
        await UniTask.Delay(System.TimeSpan.FromSeconds(seconds));
        _isAssignToAttack = true;
    }
}

