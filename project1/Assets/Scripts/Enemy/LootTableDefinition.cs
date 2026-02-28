using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/전리품/드랍 테이블")]
public class LootTableDefinition : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [Header("드랍될 오브젝트 프리펩")]
        public GameObject dropPrefab;

        [Header("드랍 확률")] [Range(0f, 1f)] public float chance = 0.3f;

        [Header("드랍 수량")] 
        public int minAmount = 1;
        public int maxAmount = 1;

        [Header("드랍 위치 랜덤 반경")]
        public float scatterRadius = 0.6f;
    }
    public Entry[] entries;
    
    [Header("최소 드랍 보장")]
    public bool guaranteeAtLeastOne = false;
}
