using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("플레이어")]
    [SerializeField] private PlayerController playerController;

    [Header("리소스")]
    [SerializeField] private ResourceInventory inventory;
    [SerializeField] private ResourceDefinition woodResource;
    [SerializeField] private ResourceDefinition stoneResource;
    [SerializeField] private ResourceDefinition ironResource;
    [SerializeField] private ResourceDefinition coinResource;

    private VisualElement root;
    private VisualElement hpFill;
    private Label hpValue;
    private VisualElement xpFill;
    private Label xpValue;
    private Label resWood, resStone, resIron, resCoin;

    private const float MaxHp = 100f;

    private void Start()
    {
        root = uiDocument.rootVisualElement;

        hpFill  = root.Q<VisualElement>("hp-fill");
        hpValue = root.Q<Label>("hp-value");
        xpFill  = root.Q<VisualElement>("xp-fill");
        xpValue = root.Q<Label>("xp-value");

        resWood  = root.Q<Label>("res-wood");
        resStone = root.Q<Label>("res-stone");
        resIron  = root.Q<Label>("res-iron");
        resCoin  = root.Q<Label>("res-coin");

        if (inventory == null)
            inventory = FindFirstObjectByType<ResourceInventory>();
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (inventory != null)
            inventory.OnChanged += UpdateResourceUI;
        if (playerController != null)
            playerController.OnHPChanged += UpdateHP;

        UpdateResourceUI();
        UpdateHP(MaxHp, MaxHp);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChanged -= UpdateResourceUI;
        if (playerController != null)
            playerController.OnHPChanged -= UpdateHP;
    }

    private void UpdateHP(float currentHP, float maxHP)
    {
        float ratio = Mathf.Clamp01(currentHP / MaxHp);
        hpFill.style.width = Length.Percent(ratio * 100f);
        hpValue.text = $"{Mathf.RoundToInt(currentHP)} / {Mathf.RoundToInt(MaxHp)}";
    }

    public void UpdateXP(int current, int max)
    {
        float ratio = Mathf.Clamp01((float)current / max);
        xpFill.style.width = Length.Percent(ratio * 100f);
        xpValue.text = $"{current} / {max}";
    }

    private void UpdateResourceUI()
    {
        if (inventory == null) return;
        resWood.text  = inventory.Get(woodResource).ToString();
        resStone.text = inventory.Get(stoneResource).ToString();
        resIron.text  = inventory.Get(ironResource).ToString();
        resCoin.text  = inventory.Get(coinResource).ToString();
    }
}