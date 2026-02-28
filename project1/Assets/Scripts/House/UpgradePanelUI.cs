using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text infoText;

    private HouseSystem _houseSystem;

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        closeButton.onClick.AddListener(Close);
    }

    public void Open(HouseSystem houseSystem)
    {
        _houseSystem = houseSystem;
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        if (_houseSystem.CanUpgrade(out var nextDef))
        {
            infoText.text = $"업그레이드 가능 → Lv{nextDef.level}";
            upgradeButton.interactable = true;
        }
        else
        {
            infoText.text = "재료 부족 또는 최대 레벨";
            upgradeButton.interactable = false;
        }
    }

    private void OnUpgradeClicked()
    {
        if (_houseSystem.TryUpgrade())
        {
            Refresh();
        }
    }
}