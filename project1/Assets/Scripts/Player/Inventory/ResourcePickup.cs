using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceDefinition resource;
    [SerializeField] private int amount = 1;
    
    [SerializeField] private InventoryItemBase inventoryItemPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        var resInv = other.GetComponent<ResourceInventory>();
        if (resInv != null)
            resInv.Add(resource, amount);
        
        var inv = other.GetComponent<Inventory>();
        if (inv != null && inventoryItemPrefab != null)
        {
            InventoryItemBase item = Instantiate(inventoryItemPrefab);
            item.gameObject.SetActive(false);
            inv.AddItem(item);
        }
        if (resource.resourceType == ResourceType.Coin)
            SoundManager.Instance.PlayCoinPickup();
        else
        SoundManager.Instance.PlayItemPickup();
        
        Destroy(gameObject);
    }
}