using UnityEngine;

public class HouseHealZone : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private HouseSystem houseSystem;
    [SerializeField] private PlayerController playerHp;

    [Header("레벨별 초당 회복량")]
    [SerializeField] private float[] healPerSecondByLevel = { 0f, 2f, 4f, 7f };
    
    private int _insideCount;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        if (_insideCount <= 0) return;
        if (playerHp == null || houseSystem == null) return;

        float hps = GetHealPerSecond(houseSystem.CurrentLevel);
        if (hps <= 0f) return;

        playerHp.Heal(hps * Time.deltaTime);
    }

    private float GetHealPerSecond(int level)
    {
        if (healPerSecondByLevel == null || healPerSecondByLevel.Length == 0) return 0f;
        level = Mathf.Clamp(level, 0, healPerSecondByLevel.Length - 1);
        return healPerSecondByLevel[level];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _insideCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _insideCount = Mathf.Max(0, _insideCount - 1);
    }
}