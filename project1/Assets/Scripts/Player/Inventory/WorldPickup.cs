using UnityEngine;

public class WorldPickup : MonoBehaviour
{
    [Header("인벤에 들어갈 자원")]
    [SerializeField] private ResourceDefinition resource;
    
    [Header("지급량")]
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))return;
        
        var inv = other.GetComponent<ResourceInventory>();
        if(inv == null) return;
        
        inv.Add(resource, amount);
        Destroy(gameObject);
    }
}
