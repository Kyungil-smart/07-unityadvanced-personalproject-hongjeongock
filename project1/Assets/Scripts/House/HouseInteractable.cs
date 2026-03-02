using UnityEngine;

public class HouseInteractable : MonoBehaviour
{
    [SerializeField] private HouseSystem houseSystem;
    [SerializeField] private UpgradePanelUI upgradePanel;
    [SerializeField] private GameObject eHintUI;

    private bool _playerInRange;

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[E] pressed - opening panel");
            upgradePanel.Open(houseSystem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ENTER] {other.name} tag={other.tag} id={other.GetInstanceID()}");

        if (!other.CompareTag("Player")) return;

        _playerInRange = true;
        if (eHintUI != null) eHintUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[EXIT] {other.name} tag={other.tag} id={other.GetInstanceID()}");

        if (!other.CompareTag("Player")) return;

        _playerInRange = false;
        if (eHintUI != null) eHintUI.SetActive(false);
    }
}