// WeaponData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("무기 기본 정보")]
    public string weaponName;        // "야구배트", "단검" 등
    public string description;       // 무기 설명
    public Sprite icon;              // 핫바 아이콘
    public GameObject weaponPrefab;  // 3D 모델 (완성되면 연결)
    public WeaponType weaponType;

    [Header("기본 스탯")]
    public float baseDamage;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float baseKnockback;

    [Header("업그레이드 옵션 풀 (5개)")]
    public List<UpgradeOptionData> upgradeOptions; // 여기서 랜덤 3개 뽑기
    
    [Header("상인 구매 가격")]
    public int price;
}

public enum WeaponType
{
    Melee,   // 근접 (야구배트, 단검, 해머)
    Ranged,  // 원거리 (석궁, 새총)
    Gun      // 총 (상인 전용)
}