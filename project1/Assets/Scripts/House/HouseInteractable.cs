using UnityEngine;
using UnityEngine.UIElements;

public class HouseInteractable : MonoBehaviour
{
    [SerializeField] private HouseSystem houseSystem;
    [SerializeField] private GameObject eHintUI;
    [SerializeField] private HouseUpgradeUI houseUpgradeUI;

    private bool _playerInRange;

    private void Update()
    {
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (houseUpgradeUI != null) 
                houseUpgradeUI.Show();
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