using UnityEngine;

/// <summary>
/// 인벤토리에 들어가는 모든 아이템의 베이스 클래스
/// </summary>
public class InventoryItemBase : MonoBehaviour
{
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Sprite icon;
    [SerializeField] private string description = "";

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public string Description => description;
}
