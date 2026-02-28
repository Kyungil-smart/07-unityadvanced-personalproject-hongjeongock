using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceDefinition resource;
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))return;
        
        var inv = other.GetComponent<ResourceInventory>();
        if (inv != null)return;
        
        inv.Add(resource, amount);
        Destroy(gameObject);
    }
}
