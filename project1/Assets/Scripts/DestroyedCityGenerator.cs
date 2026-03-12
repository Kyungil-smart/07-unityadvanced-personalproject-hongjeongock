using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도시 맵 생성기
///
/// [Inspector 설정 - 이것만 하면 됨]
/// 1. groundPrefabs   → 바닥 타일 드래그
/// 2. buildingPrefabs → 건물 Prefab 드래그
/// 3. propPrefabs     → 소품/잔해 드래그 (선택)
/// 4. tileSize        → 바닥 타일 1개 크기(m)
/// 5. Play!
///
/// [건물이 땅에 묻히면]
/// → buildingYOffset 을 양수로 조금씩 올리기
///
/// [바닥이 너무 높으면]
/// → groundYOffset 을 음수로 내리기
/// </summary>
public class CityMapGenerator : MonoBehaviour
{
    [Header("── 맵 크기 ──")]
    public int   mapWidth       = 20;   // 가로 칸 수
    public int   mapHeight      = 20;   // 세로 칸 수
    public float cellSize       = 10f;  // 칸 하나의 크기(m)

    [Header("── 바닥 ──")]
    public GameObject[] groundPrefabs;
    [Tooltip("바닥 타일 1개 크기(m). 1x1=1 / 2x2=2 / 4x4=4")]
    public float tileSize       = 1f;
    [Tooltip("바닥 Y 위치. 바닥이 높으면 음수로 내리기")]
    public float groundYOffset  = 0f;

    [Header("── 건물 ──")]
    public GameObject[] buildingPrefabs;
    [Tooltip("건물이 땅에 묻히면 양수로 올리기")]
    public float buildingYOffset = 0f;
    [Tooltip("건물 배치 확률 (0~1). 낮을수록 텅 빈 도시")]
    [Range(0f, 1f)]
    public float buildingChance  = 0.6f;

    [Header("── 소품/잔해 ──")]
    public GameObject[] propPrefabs;
    [Tooltip("소품 배치 확률")]
    [Range(0f, 1f)]
    public float propChance      = 0.15f;
    [Tooltip("소품 Y 위치")]
    public float propYOffset     = 0f;

    [Header("── 시드 ──")]
    public int seed = 42;

    // ─────────────────────────────────────────
    private List<GameObject> spawned = new List<GameObject>();

    void Start() => Generate();

    public void Generate()
    {
        Clear();
        Random.InitState(seed);

        PlaceGround();
        PlaceBuildings();
        PlaceProps();

        Debug.Log($"[CityGen] {mapWidth}x{mapHeight} 완료");
    }

    public void Clear()
    {
        foreach (var o in spawned)
            if (o != null) DestroyImmediate(o);
        spawned.Clear();
    }

    // ─────────────────────────────────────────
    // 1. 바닥: 전체 맵을 타일로 꽉 채움
    // ─────────────────────────────────────────
    void PlaceGround()
    {
        if (groundPrefabs == null || groundPrefabs.Length == 0) return;

        float ts     = Mathf.Max(0.01f, tileSize);
        float totalW = mapWidth  * cellSize;
        float totalD = mapHeight * cellSize;

        int cntX = Mathf.Max(1, Mathf.RoundToInt(totalW / ts));
        int cntZ = Mathf.Max(1, Mathf.RoundToInt(totalD / ts));

        for (int ix = 0; ix < cntX; ix++)
        {
            for (int iz = 0; iz < cntZ; iz++)
            {
                Vector3 pos = transform.position + new Vector3(
                    ix * ts + ts * 0.5f,
                    groundYOffset,
                    iz * ts + ts * 0.5f);

                var prefab = groundPrefabs[Random.Range(0, groundPrefabs.Length)];
                Do(prefab, pos, Quaternion.Euler(0, Random.Range(0,4)*90f, 0));
            }
        }
    }

    // ─────────────────────────────────────────
    // 2. 건물: 칸마다 하나씩, 겹침 없음
    //    각 셀 중앙에 하나 배치 → 절대 겹치지 않음
    // ─────────────────────────────────────────
    void PlaceBuildings()
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0) return;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                // buildingChance 확률로만 배치
                if (Random.value > buildingChance) continue;

                // 셀 중앙
                Vector3 pos = transform.position + new Vector3(
                    x * cellSize + cellSize * 0.5f,
                    buildingYOffset,
                    z * cellSize + cellSize * 0.5f);

                var prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                float rotY = Random.Range(0, 4) * 90f;  // 기울기 없이 Y축만 회전
                Do(prefab, pos, Quaternion.Euler(0, rotY, 0));
            }
        }
    }

    // ─────────────────────────────────────────
    // 3. 소품: 셀 가장자리 근처에 랜덤 배치
    // ─────────────────────────────────────────
    void PlaceProps()
    {
        if (propPrefabs == null || propPrefabs.Length == 0) return;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                if (Random.value > propChance) continue;

                // 셀 가장자리 근처 랜덤 위치
                float edgeOffset = cellSize * 0.35f;
                Vector3 center = transform.position + new Vector3(
                    x * cellSize + cellSize * 0.5f,
                    propYOffset,
                    z * cellSize + cellSize * 0.5f);

                Vector3 pos = center + new Vector3(
                    Random.Range(-edgeOffset, edgeOffset),
                    0,
                    Random.Range(-edgeOffset, edgeOffset));

                var prefab = propPrefabs[Random.Range(0, propPrefabs.Length)];
                Do(prefab, pos, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
        }
    }

    // ─────────────────────────────────────────
    GameObject Do(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return null;
        var obj = Instantiate(prefab, pos, rot, transform);
        spawned.Add(obj);
        return obj;
    }

    void OnDrawGizmosSelected()
    {
        float w = mapWidth  * cellSize;
        float d = mapHeight * cellSize;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            transform.position + new Vector3(w*0.5f, 1f, d*0.5f),
            new Vector3(w, 2f, d));
    }
}