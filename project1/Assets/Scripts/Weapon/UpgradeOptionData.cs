using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeOption", menuName = "Game/Upgrade Option")]
public class UpgradeOptionData : ScriptableObject
{
    [Header("옵션 기본 정보")]
    public string optionName;        // 옵션 이름
    public string description;       // 설명 텍스트
    public Sprite icon;

    [Header("효과")]
    public UpgradeType upgradeType;  // 어떤 스탯을 올릴지
    public float value;              // 증가량
}

public enum UpgradeType
{
    AttackDamage,      // 공격력
    AttackSpeed,       // 공격속도
    AttackRange,       // 공격 범위
    CriticalChance,    // 치명타 확률
    Knockback,         // 넉백
    Stun,              // 스턴 확률
    DotDamage,         // 지속 데미지 (출혈/독)
    Piercing,          // 관통
    SlowEffect,        // 이동속도 감소
    DamageReduction,   // 피격 데미지 감소
    MultiShot,         // 멀티샷
    AreaDamage,        // 범위 데미지
}