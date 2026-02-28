using System;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    private readonly Dictionary<string, int> _amounts = new();
    
    public event Action<string, int> OnChanged;

    public int GetAmount(ResourceDefinition def)
    {
        if(def == null) return 0;
        return _amounts.TryGetValue(def.id, out var amount) ? amount : 0;
    }

    public void Add(ResourceDefinition def, int amount)
    {
        if(def == null || amount <= 0) return;
        
        int current = GetAmount(def);
        int next = current + amount;
        _amounts[def.id] = next;
        
        OnChanged?.Invoke(def.id, next);
    }

    public bool Has(ResourceDefinition def, int required)
    {
        if(def == null) return  false;
        return GetAmount(def) >= required;
    }

    public bool Spend(ResourceDefinition def, int amount)
    {
        if (!Has(def, amount)) return false;
        
        int next = GetAmount(def) -  amount;
        _amounts[def.id] = next;
        OnChanged?.Invoke(def.id, next);
        return true;
    }
}
