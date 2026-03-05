using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceHUD : MonoBehaviour
{
    [Header("의존성")]
    [SerializeField] private ResourceInventory inventory;
    [SerializeField] private ResourceDefinition[] definitions;

    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (uiDocument == null || inventory == null) return;
        var root = uiDocument.rootVisualElement;

        foreach (var def in definitions)
        {
            if (def == null) continue;
            var label = root.Q<Label>("res-" + def.id);
            if (label != null)
                label.text = inventory.GetAmount(def).ToString();
        }
    }
}