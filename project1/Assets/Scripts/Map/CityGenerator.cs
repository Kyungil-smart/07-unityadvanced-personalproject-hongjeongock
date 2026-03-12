using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 3D 도시 생성기 v4 - 잔디 타일 + 건물만
/// 도로는 직접 배치
/// </summary>
public class CityGenerator : MonoBehaviour
{
    [Header("그리드 설정")]
    public float tileSize   = 10f;  // 타일 1개 크기
    public int   gridCountX = 20;   // 가로 타일 수
    public int   gridCountZ = 20;   // 세로 타일 수

    [Header("바닥 타일 프리팹")]
    public GameObject[] grassPrefabs;

    [Header("건물 프리팹")]
    public GameObject[] buildingPrefabs;
    [Range(0f, 1f)]
    public float buildingDensity = 0.3f;  // 낮을수록 건물 적음

    [Header("랜덤 시드")]
    public int  seed          = 42;
    public bool useRandomSeed = false;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Transform        cityParent;

    void Start() => GenerateCity();

    public void GenerateCity()
    {
        if (useRandomSeed) seed = Random.Range(0, 99999);
        Random.InitState(seed);

        ClearCity();
        cityParent = new GameObject("=== City ===").transform;
        cityParent.SetParent(transform);

        Transform grassParent = new GameObject("-- Ground --").transform;
        Transform buildParent = new GameObject("-- Buildings --").transform;
        grassParent.SetParent(cityParent);
        buildParent.SetParent(cityParent);

        for (int gz = 0; gz < gridCountZ; gz++)
        {
            for (int gx = 0; gx < gridCountX; gx++)
            {
                float px = gx * tileSize;
                float pz = gz * tileSize;
                float py = GetGroundHeight(px, pz);

                // 잔디 타일 (살짝 겹치게 tileSize + 0.1f 간격)
                PlaceGrass(new Vector3(px, py, pz), grassParent);

                // 건물 - 타일 중심에 1개만 배치 (겹침 방지)
                if (Random.value <= buildingDensity)
                    PlaceBuilding(new Vector3(px, py, pz), buildParent);
            }
        }

        Debug.Log($"[CityGenerator] 생성 완료 | Seed:{seed} | 오브젝트:{spawnedObjects.Count}개");
    }

    void PlaceGrass(Vector3 pos, Transform parent)
    {
        if (grassPrefabs == null || grassPrefabs.Length == 0) return;
        var prefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)];
        if (prefab == null) return;
        var obj = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0), parent);
        // 프리팹 원본 크기 그대로 (겹침 없음)
        obj.transform.localScale = Vector3.one;
        spawnedObjects.Add(obj);
    }

    void PlaceBuilding(Vector3 pos, Transform parent)
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            PlaceDefaultBuilding(pos, parent); return;
        }
        var prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        if (prefab == null) return;
        var b = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0,4)*90f, 0), parent);
        b.transform.localScale = Vector3.one;
        b.name = $"Building_{spawnedObjects.Count}";
        spawnedObjects.Add(b);
    }

    void PlaceDefaultBuilding(Vector3 pos, Transform parent)
    {
        var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.transform.SetParent(parent);
        float w = Random.Range(2f, tileSize * 0.7f);
        float h = Random.Range(3f, 12f);
        float d = Random.Range(2f, tileSize * 0.7f);
        b.transform.position   = pos + Vector3.up * (h * 0.5f);
        b.transform.localScale = new Vector3(w, h, d);
        b.GetComponent<Renderer>().material.color = GetBuildingColor();
        b.name = $"DefaultBuilding_{spawnedObjects.Count}";
        spawnedObjects.Add(b);
    }

    float GetGroundHeight(float x, float z)
    {
        int mask = LayerMask.GetMask("Ground");
        Ray ray  = new Ray(new Vector3(x, 1000f, z), Vector3.down);
        if (mask != 0)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f, mask))
                return hit.point.y;
        }
        else
        {
            foreach (var hit in Physics.RaycastAll(ray, 2000f))
            {
                if (cityParent != null && hit.transform.IsChildOf(cityParent)) continue;
                return hit.point.y;
            }
        }
        return 0f;
    }

    Color GetBuildingColor()
    {
        Color[] p = {
            new Color(0.6f,0.6f,0.65f), new Color(0.7f,0.55f,0.4f),
            new Color(0.4f,0.5f,0.65f), new Color(0.8f,0.75f,0.6f),
            new Color(0.3f,0.35f,0.3f),
        };
        return p[Random.Range(0, p.Length)];
    }

    void ClearCity()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
        if (cityParent != null)
        {
            if (Application.isPlaying) Destroy(cityParent.gameObject);
            else DestroyImmediate(cityParent.gameObject);
        }
    }

    [ContextMenu("도시 재생성")] void RegenerateCity() => GenerateCity();
    [ContextMenu("도시 삭제")]   void DeleteCity()      => ClearCity();
}
