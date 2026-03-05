using System;
using UnityEngine;

public class HouseSystem : MonoBehaviour
{
    [SerializeField] private ResourceInventory playerInventory;
    [SerializeField] private ResourceInventory upgradeStorage;

    [Header("집 업그레이드 데이터")]
    [SerializeField] private UpgradeItemData houseUpgradeData;

    [Header("레벨별 집 프리펩")]
    [SerializeField] private GameObject[] housePrefabs;

    [Header("집 생성 위치")]
    [SerializeField] private Transform houseRoot;

    [Header("업그레이드 시 플레이어 위치")]
    [SerializeField] private Transform upgradeSafePoint;

    [Header("현재 집 레벨")]
    [SerializeField] private int currentLevel = 0;

    private GameObject _currentHouseInstance;

    public int CurrentLevel
    {
        get => currentLevel;
        private set => currentLevel = value;
    }
    public event Action<int> OnUpgraded;

    private void Start()
    {
        EnsureInitialHouse();
    }

    private void ReplaceHouse(GameObject nextHousePrefab)
    {
        if (houseRoot == null)
        {
            Debug.LogError("[HouseSystem] houseRoot가 비어있음");
            return;
        }
        if (nextHousePrefab == null)
        {
            Debug.LogError("[HouseSystem] nextHousePrefab이 null");
            return;
        }

        // ✅ 프리팹이 가진 스케일(네가 원하는 크기) 기억
        Vector3 prefabScale = nextHousePrefab.transform.localScale;

        // ✅ 겹침 방지: houseRoot 아래 기존 집 전부 제거
        for (int i = houseRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(houseRoot.GetChild(i).gameObject);
        }

        // ✅ 새 집 생성
        _currentHouseInstance = Instantiate(nextHousePrefab);
        _currentHouseInstance.transform.SetParent(houseRoot, true);

        // ✅ 위치/회전은 houseRoot 기준으로 (원하는 고정 위치)
        _currentHouseInstance.transform.position = houseRoot.position;
        _currentHouseInstance.transform.rotation = houseRoot.rotation;

        // ✅ 스케일은 프리팹 값 그대로 복원 (네 스케일 “절대” 바꾸지 않음)
        _currentHouseInstance.transform.localScale = prefabScale;
    }

    private void EnsureInitialHouse()
    {
        if (houseRoot == null)
        {
            Debug.LogError("[HouseSystem] houseRoot가 비어있음");
            return;
        }
        
        if (houseRoot.childCount > 0)
        {
            _currentHouseInstance = houseRoot.GetChild(0).gameObject;
            CurrentLevel = currentLevel;
            return;
        }
        
        if (housePrefabs == null || housePrefabs.Length == 0 || housePrefabs[0] == null)
        {
            Debug.LogError("[HouseSystem] housePrefabs[0] (레벨0 집 프리팹)이 없음");
            return;
        }

        ReplaceHouse(housePrefabs[0]);
        CurrentLevel = 0;
    }

    public bool TryUpgradeHouse()
    {
        if (houseUpgradeData == null) return false;

        int nextLevel = currentLevel + 1;
        
        if (nextLevel > houseUpgradeData.maxLevel)
        {
            Debug.Log("[HouseSystem] 이미 최대 레벨입니다.");
            return false;
        }
        
        if (housePrefabs == null || nextLevel < 0 || nextLevel >= housePrefabs.Length || housePrefabs[nextLevel] == null)
        {
            Debug.Log("[HouseSystem] 다음 레벨 프리팹이 없음");
            return false;
        }

        int costIndex = nextLevel - 1;

        int woodCost = GetCostSafe(houseUpgradeData.woodCost, costIndex);
        int stoneCost = GetCostSafe(houseUpgradeData.stoneCost, costIndex);
        int ironCost = GetCostSafe(houseUpgradeData.ironCost, costIndex);
        int coinCost = GetCostSafe(houseUpgradeData.coinCost, costIndex);

        if (!HasCost(houseUpgradeData.woodResource, woodCost)) return false;
        if (!HasCost(houseUpgradeData.stoneResource, stoneCost)) return false;
        if (!HasCost(houseUpgradeData.ironResource, ironCost)) return false;
        if (!HasCost(houseUpgradeData.coinResource, coinCost)) return false;

        SpendCost(houseUpgradeData.woodResource, woodCost);
        SpendCost(houseUpgradeData.stoneResource, stoneCost);
        SpendCost(houseUpgradeData.ironResource, ironCost);
        SpendCost(houseUpgradeData.coinResource, coinCost);

        MovePlayerToSafePointIfPossible();
        
        ReplaceHouse(housePrefabs[nextLevel]);

        currentLevel = nextLevel;
        OnUpgraded?.Invoke(currentLevel);

        Debug.Log($"[HouseSystem] House upgraded -> Level {currentLevel}");
        return true;
    }

    private bool HasCost(ResourceDefinition res, int amount)
    {
        if (amount <= 0) return true;
        if (res == null) return true;
        if (upgradeStorage == null) return false;

        return upgradeStorage.Has(res, amount);
    }

    private void SpendCost(ResourceDefinition res, int amount)
    {
        if (amount <= 0) return;
        if (res == null) return;
        if (upgradeStorage == null) return;

        upgradeStorage.Spend(res, amount);
    }

    private int GetCostSafe(int[] costs, int index)
    {
        if (costs == null) return 0;
        if (index < 0 || index >= costs.Length) return 0;
        return costs[index];
    }

    private void ApplyHouseVisualForLevel(int level)
    {
        if (houseRoot == null)
        {
            Debug.LogError("[HouseSystem] houseRoot가 비어있습니다.");
            return;
        }
        
        for (int i = houseRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(houseRoot.GetChild(i).gameObject);
        }
        
        if (level <= 0)
        {
            _currentHouseInstance = null;
            return;
        }

        if (housePrefabs == null || housePrefabs.Length == 0)
        {
            Debug.LogError("[HouseSystem] housePrefabs가 비어있습니다.");
            return;
        }
        
        int index = Mathf.Clamp(level - 1, 0, housePrefabs.Length - 1);

        var prefab = housePrefabs[index];
        if (prefab == null)
        {
            Debug.LogError($"[HouseSystem] housePrefabs[{index}]가 비어있습니다.");
            return;
        }

        _currentHouseInstance = Instantiate(prefab, houseRoot);
        _currentHouseInstance.transform.localPosition = Vector3.zero;
        _currentHouseInstance.transform.localRotation = Quaternion.identity;
    }

    private void MovePlayerToSafePointIfPossible()
    {
        if (upgradeSafePoint == null) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = upgradeSafePoint.position;
        player.transform.rotation = upgradeSafePoint.rotation;

        if (cc != null) cc.enabled = true;
    }

    public bool TryDepositToStorage(ResourceDefinition resource, int amount)
    {
        if (amount <= 0) return false;
        if (playerInventory == null || upgradeStorage == null) return false;
        if (!playerInventory.Has(resource, amount)) return false;

        playerInventory.Spend(resource, amount);
        upgradeStorage.Add(resource, amount);
        return true;
    }
}