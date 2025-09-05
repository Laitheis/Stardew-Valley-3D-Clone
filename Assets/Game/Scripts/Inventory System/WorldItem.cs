using System.Collections;
using UnityEngine;
namespace InventorySystem
{
    public class WorldItem : MonoBehaviour
    {
        [SerializeField] private float _attractionDelay = 1f;
        [SerializeField] private float _itemAttractSpeed = 5f;
        [SerializeField] private float _attractionRadius = 5f;

        public ItemInstance ItemInstance;

        public ItemDefinition Item => ItemInstance.ItemDefinition;

        private void Start()
        {
            StartCoroutineAttractionItem();
        }
        public void StartCoroutineAttractionItem()
        {
            StartCoroutine(AttractingItem(gameObject));
        }
        private IEnumerator AttractingItem(GameObject item)
        {
            yield return new WaitForSeconds(_attractionDelay);

            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            while (item != null && Vector3.Distance(item.transform.position, player.position) > 2f && player != null)
            {
                float distance = Vector3.Distance(item.transform.position, player.position);
                if (distance <= _attractionRadius)
                {
                    Vector3 direction = (player.position - item.transform.position).normalized;
                    itemRb.velocity = direction * _itemAttractSpeed; // Use velocity for movement
                }

                yield return null;
            }
        }
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.tag == "Player")
        //    {
        //        collision.transform.GetComponentInChildren<InventoryController>().AddFromWorld(this);
        //    }
        //}
    }
}
