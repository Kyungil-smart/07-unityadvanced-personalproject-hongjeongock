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

        var levelSystem = other.GetComponent<PlayerLevelSystem>();
        if (levelSystem != null)
        {
            int gainedXP = GetResourceXP() * amount;
            levelSystem.AddXP(gainedXP);
        }

        Destroy(gameObject);
    }

    private int GetResourceXP()
    {
        if (resource == null) return 0;

        string resourceName = resource.name.ToLower();

        if (resourceName.Contains("iron"))
            return 2;

        if (resourceName.Contains("stone"))
            return 1;

        if (resourceName.Contains("wood"))
            return 1;

        return 1;
    }
}