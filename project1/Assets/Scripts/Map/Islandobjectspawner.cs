using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 모든 섬에 범용으로 사용 가능한 오브젝트 랜덤 배치 스폰러
/// 섬 오브젝트에 컴포넌트로 추가하고 프리팹 배열만 채우면 됨
/// </summary>
public class IslandObjectSpawner : MonoBehaviour
{
    [Header("스폰 범위")]
    [SerializeField] private float islandRadius = 50f;
    [SerializeField] private float edgeMargin = 3f;
    [SerializeField] private float noSpawnRadius = 5f;       // 중앙(입구) 배치 금지
    [SerializeField] private LayerMask terrainLayer;

    [Header("기즈모 위치 오프셋")]
    [SerializeField] private Vector3 gizmoOffset = Vector3.zero;  // X: 좌우, Z: 앞뒤

    [Header("배치 규칙")]
    [SerializeField] private float minDistanceBetweenObjects = 2f;
    [SerializeField] private int maxAttempts = 30;

    [Header("시드 (0 = 매번 랜덤)")]
    [SerializeField] private int seed = 0;

    [Header("스폰 그룹 목록")]
    [SerializeField] private SpawnGroup[] spawnGroups;

    private List<Vector3> _placedPositions = new List<Vector3>();

    private void Start()
    {
        if (seed != 0)
            Random.InitState(seed);

        foreach (var group in spawnGroups)
        {
            if (!group.enabled) continue;
            SpawnGroup(group);
        }
    }

    private void SpawnGroup(SpawnGroup group)
    {
        if (group.prefabs == null || group.prefabs.Length == 0) return;

        int spawned = 0;

        for (int i = 0; i < group.count; i++)
        {
            if (!TryGetValidPosition(group.customNoSpawnRadius > 0
                    ? group.customNoSpawnRadius
                    : noSpawnRadius, out Vector3 pos)) continue;

            GameObject prefab = group.prefabs[Random.Range(0, group.prefabs.Length)];
            if (prefab == null) continue;

            // 부모 Transform 결정 (지정 없으면 this)
            Transform parent = group.parentTransform != null ? group.parentTransform : transform;
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);

            // 랜덤 Y축 회전
            if (group.randomRotation)
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            // 랜덤 크기
            float scale = Random.Range(group.minScale, group.maxScale);
            obj.transform.localScale = Vector3.one * scale;

            _placedPositions.Add(pos);
            spawned++;
        }

        Debug.Log($"[IslandObjectSpawner] '{group.groupName}' {spawned}/{group.count}개 배치 완료");
    }

    private bool TryGetValidPosition(float centerExcludeRadius, out Vector3 result)
    {
        result = Vector3.zero;
        Vector3 center = transform.position;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 rand2D = Random.insideUnitCircle * (islandRadius - edgeMargin);
            Vector3 candidate = center + new Vector3(rand2D.x, 100f, rand2D.y);

            // 중앙 금지 구역 체크
            float distFromCenter = Vector2.Distance(
                new Vector2(candidate.x, candidate.z),
                new Vector2(center.x, center.z));
            if (distFromCenter < centerExcludeRadius) continue;

            // 지형 Raycast
            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit hit, 200f, terrainLayer))
            {
                Vector3 pos = hit.point;
                if (!IsTooClose(pos))
                {
                    result = pos;
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsTooClose(Vector3 pos)
    {
        foreach (var p in _placedPositions)
            if (Vector3.Distance(pos, p) < minDistanceBetweenObjects) return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + gizmoOffset;
        float boxHeight = islandRadius * 0.5f;

        // 스폰 범위 - 초록 3D 박스 (반투명 채움 + 외곽선)
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawCube(center, new Vector3(islandRadius * 2, boxHeight, islandRadius * 2));
        Gizmos.color = new Color(0, 1, 0, 0.8f);
        Gizmos.DrawWireCube(center, new Vector3(islandRadius * 2, boxHeight, islandRadius * 2));

        // 중앙 배치 금지 구역 - 빨간 3D 박스
        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawCube(center, new Vector3(noSpawnRadius * 2, boxHeight, noSpawnRadius * 2));
        Gizmos.color = new Color(1, 0, 0, 0.8f);
        Gizmos.DrawWireCube(center, new Vector3(noSpawnRadius * 2, boxHeight, noSpawnRadius * 2));
    }
}

/// <summary>
/// 스폰 그룹 하나 (나무, 바위, 좀비 등 각각 설정)
/// </summary>
[System.Serializable]
public class SpawnGroup
{
    [Header("그룹 정보")]
    public string groupName = "새 그룹";
    public bool enabled = true;

    [Header("프리팹")]
    public GameObject[] prefabs;

    [Header("수량 및 크기")]
    public int count = 20;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("옵션")]
    public bool randomRotation = true;
    public float customNoSpawnRadius = 0f;  // 0이면 전역 설정 사용
    public Transform parentTransform;       // 비워두면 섬 오브젝트 하위로
}