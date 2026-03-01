using System;
using TMPro;
using UnityEngine;

public class HouseUpgradeInteractor : MonoBehaviour
{
    [Header("플레이어 판별")]
    [SerializeField] private string playerTag = "Player";
    
    [Header("UI")]
    [SerializeField] private GameObject eHintUI;
    [SerializeField] private GameObject upgradePanelRoot;

    [SerializeField] private HouseSystem houseSystem;
    [SerializeField] private UpgradePanelUI upgradePanelUI;

    private bool _inZone;

    private void Awake()
    {
        if(eHintUI != null) eHintUI.SetActive(false);
        if(upgradePanelRoot != null) upgradePanelRoot.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER!!");
        if(!other.CompareTag(playerTag))return;
        
        _inZone = true;
        
        if(eHintUI != null) eHintUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(playerTag)) return;
        _inZone = false;
        
        if(eHintUI != null) eHintUI.SetActive(false);
        
        if(upgradePanelRoot != null) upgradePanelRoot.SetActive(false);

        if (upgradePanelUI != null) upgradePanelUI.Close();
    }

   private void Update()
    {
        if(!_inZone) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (upgradePanelUI == null || houseSystem == null) return;

            // 이미 열려있으면 무시(버튼 클릭 방해 방지)
            if (upgradePanelUI.gameObject.activeSelf) return;

            upgradePanelUI.Open(houseSystem);

            if (eHintUI != null) eHintUI.SetActive(false);
        }
}
}
