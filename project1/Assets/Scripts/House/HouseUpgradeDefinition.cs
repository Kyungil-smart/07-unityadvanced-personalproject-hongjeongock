using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/House Upgrade Definition")]
public class HouseUpgradeDefinition : ScriptableObject
{
    [Serializable]
    public struct Cost
    {
        public ResourceDefinition resource;
        public int amount;
    }

    public int level;
    public Cost[] costs;
    public GameObject housePrefab;
}
