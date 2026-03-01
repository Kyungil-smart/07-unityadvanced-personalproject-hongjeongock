using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text infoText;
    
    [SerializeField] private ResourceDefinition woodResource;
    [SerializeField] private ResourceDefinition stoneResource;
    [SerializeField] private ResourceDefinition ironResource;

    [Header("입력 잠금")]
    [SerializeField] private PlayerInputGate inputGate;

    private HouseSystem _houseSystem;

    private void Start()
    {
        Close();
    }
    
    

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        closeButton.onClick.AddListener(Close);
    }

    public void Open(HouseSystem houseSystem)
    {
        _houseSystem = houseSystem;
        gameObject.SetActive(true);

        inputGate?.Lock();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);

        inputGate?.Unlock();
        _houseSystem = null;
    }

    private void Refresh()
    {
        if (_houseSystem != null && _houseSystem.CanUpgrade(out var nextDef))
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
        if (_houseSystem == null) return;

        if (_houseSystem.TryUpgrade())
            Refresh();
    }
    public void OnClickDepositWood()
    {
        if (_houseSystem == null) return;
        _houseSystem.TryDepositToStorage(woodResource, 1);
        Refresh();
    }
    public void OnClickDepositStone()
    {
        if (_houseSystem == null) return;
        _houseSystem.TryDepositToStorage(stoneResource, 1);
        Refresh();
    }
    public void OnClickDepositIron()
    {
        if (_houseSystem == null) return;
        _houseSystem.TryDepositToStorage(ironResource, 1);
        Refresh();
    }
}