using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemData", menuName = "HouseUpgrade/UpgradeItemData")]
public class UpgradeItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public Sprite icon;

    [Header("최대 레벨")] 
    public int maxLevel = 3;
    
    [Header("자원 참조")]
    public ResourceDefinition woodResource;
    public ResourceDefinition stoneResource;
    public ResourceDefinition ironResource;
    public ResourceDefinition coinResource;
    
    [Header("레벨 비용")]
    public int[] woodCost;
    public int[] stoneCost;
    public int[] ironCost;
    public int[] coinCost;
}
