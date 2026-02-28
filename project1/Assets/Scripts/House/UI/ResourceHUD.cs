using System.Collections.Generic;
using UnityEngine;

public class ResourceHUD : MonoBehaviour
{
    [Header("의존성")]
    [SerializeField] private ResourceInventory inventory;
    [SerializeField] private ResourceDefinition[] definitions;

    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ResourceRowUI rowPrefab;

    private readonly Dictionary<string, ResourceRowUI> _rows = new();

    private void Awake()
    {
        Build();
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnChanged += HandleChanged;
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnChanged -= HandleChanged;
    }

    private void Build()
    {
        if (contentRoot == null || rowPrefab == null || inventory == null) return;

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        _rows.Clear();

        foreach (var def in definitions)
        {
            if (def == null) continue;

            var row = Instantiate(rowPrefab, contentRoot);
            row.Bind(def, inventory.GetAmount(def));
            _rows[def.id] = row;
        }
    }

    private void HandleChanged(string id, int amount)
    {
        if (_rows.TryGetValue(id, out var row))
            row.SetAmount(amount);
    }
}