using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("HP 하트 (5개 연결)")]
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] private PlayerController playerController;

    [Header("리소스 HUD")]
    [SerializeField] private ResourceSlot[] resourceSlots;
    [SerializeField] private ResourceInventory inventory;

    private const float MaxHp = 100f;
    private const float HpPerHeart = 20f;

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<ResourceInventory>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (inventory != null)
            inventory.OnChanged += UpdateResourceUI;

        if (playerController != null)
            playerController.OnHPChanged += UpdateHearts;

        UpdateResourceUI();
        UpdateHearts(MaxHp);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChanged -= UpdateResourceUI;

        if (playerController != null)
            playerController.OnHPChanged -= UpdateHearts;
    }

    public void UpdateHearts(float currentHP)
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            float threshold = (i + 1) * HpPerHeart;
            heartImages[i].sprite = currentHP >= threshold ? heartFull : heartEmpty;
        }
    }

    public void UpdateResourceUI()
    {
        if (resourceSlots == null) return;

        foreach (var slot in resourceSlots)
        {
            if (slot.resource == null || slot.countText == null) continue;
            int amount = inventory != null ? inventory.Get(slot.resource) : 0;
            slot.countText.text = $"x{amount}";

            if (slot.iconImage != null && slot.resource.icon != null)
                slot.iconImage.sprite = slot.resource.icon;
        }
    }
}

[Serializable]
public class ResourceSlot
{
    public ResourceDefinition resource;
    public TextMeshProUGUI countText;
    public Image iconImage;
}