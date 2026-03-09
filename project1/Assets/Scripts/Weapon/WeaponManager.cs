// WeaponManager.cs
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("무기 데이터 전체 목록")]
    [SerializeField] private List<WeaponData> _allWeapons; // 인스펙터에서 5개 연결

    [Header("현재 장착 무기")]
    private WeaponData _equippedWeapon;
    
    // 현재 무기에 적용된 업그레이드 목록
    private List<UpgradeOptionData> _appliedUpgrades = new List<UpgradeOptionData>();

    // 현재 실제 스탯 (기본 스탯 + 업그레이드 합산)
    public float CurrentDamage { get; private set; }
    public float CurrentAttackSpeed { get; private set; }
    public float CurrentAttackRange { get; private set; }
    public float CurrentKnockback { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// 무기 장착 (새 게임 시작 시 호출)
    public void EquipWeapon(WeaponData weapon)
    {
        _equippedWeapon = weapon;
        _appliedUpgrades.Clear();
        RecalculateStats();

        // SaveData에 선택한 무기 저장
        PlayerPrefs.SetString("EquippedWeapon", weapon.weaponName);
        PlayerPrefs.Save();

        Debug.Log($"[WeaponManager] 무기 장착: {weapon.weaponName}");
    }

    /// 업그레이드 적용 (레벨업 시 호출)
    public void ApplyUpgrade(UpgradeOptionData upgrade)
    {
        _appliedUpgrades.Add(upgrade);
        RecalculateStats();
        Debug.Log($"[WeaponManager] 업그레이드 적용: {upgrade.optionName}");
    }

    /// 랜덤 업그레이드 옵션 3개 뽑기
    public List<UpgradeOptionData> GetRandomUpgradeOptions(int count = 3)
    {
        if (_equippedWeapon == null) return null;

        List<UpgradeOptionData> pool = new List<UpgradeOptionData>(_equippedWeapon.upgradeOptions);
        List<UpgradeOptionData> result = new List<UpgradeOptionData>();

        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex); // 중복 방지
        }

        return result;
    }

    /// 스탯 재계산
    private void RecalculateStats()
    {
        if (_equippedWeapon == null) return;

        // 기본 스탯으로 초기화
        CurrentDamage = _equippedWeapon.baseDamage;
        CurrentAttackSpeed = _equippedWeapon.baseAttackSpeed;
        CurrentAttackRange = _equippedWeapon.baseAttackRange;
        CurrentKnockback = _equippedWeapon.baseKnockback;

        // 업그레이드 합산
        foreach (var upgrade in _appliedUpgrades)
        {
            switch (upgrade.upgradeType)
            {
                case UpgradeType.AttackDamage:
                    CurrentDamage += upgrade.value;
                    break;
                case UpgradeType.AttackSpeed:
                    CurrentAttackSpeed -= upgrade.value; // 쿨타임이므로 감소
                    break;
                case UpgradeType.AttackRange:
                    CurrentAttackRange += upgrade.value;
                    break;
                case UpgradeType.Knockback:
                    CurrentKnockback += upgrade.value;
                    break;
            }
        }
    }

    /// 전체 무기 목록 반환 (무기 선택 UI용)
    public List<WeaponData> GetAllWeapons() => _allWeapons;

    /// 현재 장착 무기 반환
    public WeaponData GetEquippedWeapon() => _equippedWeapon;
}