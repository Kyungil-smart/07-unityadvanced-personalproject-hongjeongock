using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 아이템 인벤토리
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 20;

    private List<InventoryItemBase> items = new List<InventoryItemBase>();

    public IReadOnlyList<InventoryItemBase> Items => items;
    public int Count => items.Count;
    public bool IsFull => items.Count >= maxSlots;

    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    public bool AddItem(InventoryItemBase item)
    {
        if (IsFull)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        items.Add(item);
        Debug.Log($"[Inventory] {item.ItemName} 추가됨 ({items.Count}/{maxSlots})");
        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    public bool RemoveItem(InventoryItemBase item)
    {
        if (!items.Contains(item))
            return false;

        items.Remove(item);
        return true;
    }

    /// <summary>
    /// 특정 타입의 아이템 검색
    /// </summary>
    public T GetItem<T>() where T : InventoryItemBase
    {
        foreach (var item in items)
        {
            if (item is T typedItem)
                return typedItem;
        }
        return null;
    }

    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    public void Clear()
    {
        items.Clear();
    }
}
