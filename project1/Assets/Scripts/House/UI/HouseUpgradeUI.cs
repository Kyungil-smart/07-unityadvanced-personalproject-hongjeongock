using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class HouseUpgradeUI : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;
    
    [Header("ResourceInventory 참조")]
    public ResourceInventory inventory;
    
    [Header("하우스 시스템 참조")]
    [SerializeField] private HouseSystem houseSystem;

    [Header("업그레이드 아이템 데이터")]
    public UpgradeItemData houseData;
    public UpgradeItemData towerData;
    public UpgradeItemData healData;
    public UpgradeItemData zoneData;

    private int[] levels = { 1, 1, 1, 3 };

    private VisualElement root;
    private UpgradeItemData[] items;
    private string[] ids = { "house", "tower", "heal", "zone" };
    
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        if (houseSystem == null)
            houseSystem = FindFirstObjectByType<HouseSystem>();

        Debug.Log("Awake root: " + root);
    }

    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        items = new UpgradeItemData[] {houseData, towerData, healData, zoneData};

        root.Q<Button>("close-btn").clicked += OnClose;
        root.Q<Button>("btn-house").clicked += () => OnUpgrade(0);
        root.Q<Button>("btn-tower").clicked += () => OnUpgrade(1);
        root.Q<Button>("btn-heal").clicked += () => OnUpgrade(2);
        root.Q<Button>("btn-zone").clicked += () => OnUpgrade(3);

        if (inventory != null)
            inventory.OnChanged += RefreshAll;

        RefreshAll();
        Hide();
    }

    private void OnDisable()
    {
        if (root == null) return;
        
        root.Q<Button>("close-btn").clicked -= OnClose;

        if (inventory != null)
            inventory.OnChanged -= RefreshAll;
    }
    
    private void OnClose() => Hide();

    private void RefreshAll()
    {
        for (int i = 0; i < 4; i++)
        {
            RefreshCard(ids[i], i);
        }

        RefreshResourceBar();
    }

    private void RefreshCard(string id, int idx)
    {
        if (items[idx] == null) return;
        
        int lv = levels[idx];
        int maxLv = items[idx].maxLevel;
        bool isMax = lv > maxLv;
        int costIdx = lv - 1;
        
        root.Q<Label>($"level-current-{id}").text = $"Lv.{lv}";
        var nextLabel = root.Q<Label>($"level-next-{id}");
        nextLabel.text = isMax ? "Max" : $"Lv.{lv + 1}";
        nextLabel.EnableInClassList("level-max", isMax);
        nextLabel.EnableInClassList("level-next", !isMax);

        if (!isMax && costIdx < items[idx].woodCost.Length)
        {
            var data = items[idx];
            SetCost($"cost-wood-{id}", data.woodResource, data.woodCost[costIdx]);
            SetCost($"cost-stone-{id}", data.stoneResource, data.stoneCost[costIdx]);
            SetCost($"cost-iron-{id}", data.ironResource, data.ironCost[costIdx]);
            SetCost($"cost-coin-{id}", data.coinResource, data.coinCost[costIdx]);
        }
        else
        {
            SetCostDash($"cost-wood-{id}");
            SetCostDash($"cost-stone-{id}");
            SetCostDash($"cost-iron-{id}");
            SetCostDash($"cost-coin-{id}");
        }
        
        var btn = root.Q<Button>($"btn-{id}");
        if (isMax)
        {
            btn.text = "최대 레벨";
            btn.AddToClassList("upgrade-btn-maxed");
            btn.SetEnabled(false);
        }
        else
        {
            btn.text = "강화하기";
            btn.RemoveFromClassList("upgrade-btn-maxed");
            btn.SetEnabled(CanAfford(idx, costIdx));
        }
    }

    private void SetCost(string labelName, ResourceDefinition res, int amount)
    {
        var label = root.Q<Label>(labelName);
        if (label == null) return;
        label.text = amount.ToString();
        bool lack = inventory == null || !inventory.Has(res, amount);
        label.EnableInClassList("cost-amount-lack", lack);
    }

    private void SetCostDash(string labelName)
    {
        var label = root.Q<Label>(labelName);
        if (label != null)
        {
            label.text = "-";
                label.EnableInClassList("cost-amount-lack", false);
        }
    }

    bool CanAfford(int idx, int costIdx)
    {
        if (inventory == null) return false;
        var data = items[idx];
        if (data.woodCost.Length  <= costIdx) return false;
        if (data.stoneCost.Length <= costIdx) return false;
        if (data.ironCost.Length  <= costIdx) return false;
        if (data.coinCost.Length  <= costIdx) return false;
        return inventory.Has(data.woodResource,  data.woodCost[costIdx])
               && inventory.Has(data.stoneResource, data.stoneCost[costIdx])
               && inventory.Has(data.ironResource,  data.ironCost[costIdx])
               && inventory.Has(data.coinResource,  data.coinCost[costIdx]);
    }
    private void OnUpgrade(int idx)
    {
        if (idx == 0)
        {
            if (houseSystem == null)
                houseSystem = FindFirstObjectByType<HouseSystem>();

            if (houseSystem == null)
            {
                Debug.LogError("[HouseUpgradeUI] HouseSystem을 찾을 수 없습니다.");
                return;
            }

            bool ok = houseSystem.TryUpgradeHouse();
            Debug.Log($"[HouseUpgrade] 집 크기 TryUpgradeHouse => {ok}, CurrentLevel={houseSystem.CurrentLevel}");
            
            levels[0] = Mathf.Max(1, houseSystem.CurrentLevel + 1); 

            RefreshAll();
            return;
        }
        
        int lv    = levels[idx];
        int maxLv = items[idx].maxLevel;
        if (lv >= maxLv) return;

        int costIdx = lv - 1;
        if (items[idx].woodCost.Length == 0) return;
        if (!CanAfford(idx, costIdx)) return;

        var data = items[idx];
        inventory.Spend(data.woodResource,  data.woodCost[costIdx]);
        inventory.Spend(data.stoneResource, data.stoneCost[costIdx]);
        inventory.Spend(data.ironResource,  data.ironCost[costIdx]);
        inventory.Spend(data.coinResource,  data.coinCost[costIdx]);

        levels[idx]++;

        Debug.Log($"[HouseUpgrade] {data.itemName} Lv.{levels[idx]-1} → Lv.{levels[idx]}");

        RefreshAll();
    }

    private void RefreshResourceBar()
    {
        if (inventory == null || houseData == null) return;
        root.Q<Label>("res-wood").text  = inventory.Get(houseData.woodResource).ToString();
        root.Q<Label>("res-stone").text = inventory.Get(houseData.stoneResource).ToString();
        root.Q<Label>("res-iron").text  = inventory.Get(houseData.ironResource).ToString();
        root.Q<Label>("res-coin").text  = inventory.Get(houseData.coinResource).ToString();
    }
    public void Show()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        root = uiDocument.rootVisualElement;
        if (root == null) { Debug.Log("여전히 null"); return; }
        root.style.display = DisplayStyle.Flex;
        RefreshAll();
    }

    public void Hide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (root == null) return;
        root.style.display = DisplayStyle.None;
    }
}
