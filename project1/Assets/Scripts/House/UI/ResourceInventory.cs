using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    [System.Serializable]
    private class Entry
    {
        public ResourceDefinition resource;
        public int amount;
    }

    [SerializeField] private List<Entry> entries = new();

    private readonly Dictionary<ResourceDefinition, int> _map = new();

    public event Action OnChanged;

    public int GetAmount(ResourceDefinition resource) => Get(resource);

    private void Awake()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        _map.Clear();
        foreach (var e in entries)
        {
            if (e.resource == null) continue;
            _map[e.resource] = Mathf.Max(0, e.amount);
        }
        OnChanged?.Invoke();
    }

    public int Get(ResourceDefinition resource)
    {
        if (resource == null) return 0;
        return _map.TryGetValue(resource, out var v) ? v : 0;
    }

    public bool Has(ResourceDefinition resource, int amount)
    {
        if (resource == null) return false;
        return Get(resource) >= amount;
    }

    public void Add(ResourceDefinition resource, int amount)
    {
        if (resource == null || amount <= 0) return;

        int cur = Get(resource);
        _map[resource] = cur + amount;
        OnChanged?.Invoke();
    }

    public bool Spend(ResourceDefinition resource, int amount)
    {
        if (!Has(resource, amount)) return false;

        _map[resource] = Get(resource) - amount;
        OnChanged?.Invoke();
        return true;
    }
}