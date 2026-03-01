using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HouseSystem : MonoBehaviour
{
    [Header("의존성")]
    [SerializeField] private ResourceInventory playerInventory;
    [SerializeField] private ResourceInventory upgradeStorage;

    [Header("베이스 업그레이드 테이블")] 
    [SerializeField] private HouseUpgradeDefinition[] upgradeDefs;

    [Header("베이스 생성 위치")] [SerializeField] private Transform houseRoot;

    public int CurrentLevel { get; private set; } = 0;
    public event Action<int> OnUpgraded;
    
    private GameObject _currentHouseInstance;

    private void Start()
    {
        ApplyHouseVisualForLevel(CurrentLevel);
    }

    public HouseUpgradeDefinition GetNextUpgrade()
    {
        int nextLevel = CurrentLevel + 1;
        return upgradeDefs.FirstOrDefault(d => d.level == nextLevel);
    }

    public bool CanUpgrade(out HouseUpgradeDefinition nextDef)
    {
        nextDef = GetNextUpgrade();
        if (nextDef == null) return  false;

        foreach (var cost in nextDef.costs)
        {
            if(!upgradeStorage.Has(cost.resource, cost.amount))
                return false;
        }
        return true;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade(out var nextDef))
            return false;

        foreach (var cost in nextDef.costs)
        {
            upgradeStorage.Spend(cost.resource, cost.amount);
        }

        CurrentLevel = nextDef.level;

        ApplyHouseVisual(nextDef);
        
        OnUpgraded?.Invoke(CurrentLevel);

        return true;
    }
    private void ApplyHouseVisual(HouseUpgradeDefinition def)
    {
        if (def == null) return;

        if (_currentHouseInstance != null)
            Destroy(_currentHouseInstance);

        _currentHouseInstance = Instantiate(def.housePrefab, houseRoot);
    }

    private void ApplyHouseVisualForLevel(int level)
    {
        var def = upgradeDefs.FirstOrDefault(d => d.level == level);
        if (def != null)
            ApplyHouseVisual(def);
    }

    public bool TryDepositToStorage(ResourceDefinition resource, int amount)
    { 
        if(amount <= 0) return  false;
        if(playerInventory == null || upgradeStorage == null)return false;
        
        if(!playerInventory.Has(resource, amount))return  false;
        
        playerInventory.Spend(resource, amount);
        
        upgradeStorage.Add(resource, amount);
        return true;
    }
}
