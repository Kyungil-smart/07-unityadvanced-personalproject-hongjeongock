using UnityEngine;

public class EnemyLootDropper : MonoBehaviour
{
    [Header("드랍 테이블")]
    [SerializeField] private LootTableDefinition lootTable;

    [Header("드랍 기준 위치")]
    [SerializeField] private Transform dropPoint;

    private bool _dropped;
    
    public void DropLoot()
    {
        if (_dropped) return;
        _dropped = true;

        if (lootTable == null || lootTable.entries == null || lootTable.entries.Length == 0)
            return;

        Vector3 basePos = dropPoint != null ? dropPoint.position : transform.position;

        bool droppedAny = false;

        foreach (var e in lootTable.entries)
        {
            if (e.dropPrefab == null) continue;
            if (Random.value > e.chance) continue;

            int amount = Random.Range(e.minAmount, e.maxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector2 r = Random.insideUnitCircle * e.scatterRadius;
                Vector3 pos = basePos + new Vector3(r.x, 0f, r.y);

                Instantiate(e.dropPrefab, pos, Quaternion.identity);
                droppedAny = true;
            }
        }
        
        if (lootTable.guaranteeAtLeastOne && !droppedAny)
        {
            var first = lootTable.entries[0];
            if (first != null && first.dropPrefab != null)
                Instantiate(first.dropPrefab, basePos, Quaternion.identity);
        }
    }
}