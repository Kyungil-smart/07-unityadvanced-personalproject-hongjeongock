using System;
using TMPro;
using Unity.VisualScripting;
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

    private float _eBufferTimer;
    [SerializeField] private float eBufferTime = 0.2f;

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
        if(Input.GetKeyDown(KeyCode.E))
            _eBufferTimer = eBufferTime;

        if(_eBufferTimer > 0) _eBufferTimer -= Time.deltaTime;

        if(!_inZone) return;
        if(upgradePanelRoot == null || houseSystem == null) return;

        if(upgradePanelUI.gameObject.activeSelf) return;

        if(_eBufferTimer > 0)
        {
            _eBufferTimer = 0f;
            upgradePanelUI.Open(houseSystem);
            if(upgradePanelUI != null) upgradePanelRoot.SetActive(true);
        }
}
}
