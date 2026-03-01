using UnityEngine;

public class HouseInteractable : MonoBehaviour
{
    [SerializeField] private HouseSystem houseSystem;
    [SerializeField] private UpgradePanelUI upgradePanel;

    private bool _playerInRange;

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            upgradePanel.Open(houseSystem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Upgrade Zone Enter: " + other.name);
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Upgrade Zone Exit: " + other.name);
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }
}