using UnityEngine;

/// <summary>
/// Perlin Noise 기반 3D 자연 지형 생성기
/// 사용법: 빈 GameObject에 이 스크립트를 추가하고 Inspector에서 설정값 조절
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    [Header("지형 크기 설정")]
    public int width = 100;         // 가로 크기
    public int depth = 100;         // 세로 크기
    public float maxHeight = 20f;   // 최대 높이

    [Header("Perlin Noise 설정")]
    public float noiseScale = 0.05f;    // 작을수록 완만한 지형
    public int octaves = 4;             // 레이어 수 (많을수록 디테일)
    public float persistence = 0.5f;   // 각 옥타브의 강도 감소율
    public float lacunarity = 2f;      // 각 옥타브의 주파수 증가율

    [Header("랜덤 시드")]
    public int seed = 0;
    public bool useRandomSeed = true;

    [Header("머티리얼")]
    public Material terrainMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        // 시드 설정
        if (useRandomSeed)
            seed = Random.Range(0, 10000);

        // 필요한 컴포넌트 추가 (에디터/플레이 모드 모두 안전하게)
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();

        if (terrainMaterial != null && meshRenderer != null)
            meshRenderer.material = terrainMaterial;

        Mesh mesh = CreateTerrainMesh();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        Debug.Log($"[TerrainGenerator] 지형 생성 완료 | Seed: {seed} | 크기: {width}x{depth}");
    }

    Mesh CreateTerrainMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "ProceduralTerrain";

        // 정점(Vertex) 생성
        Vector3[] vertices  = new Vector3[(width + 1) * (depth + 1)];
        Vector2[] uvs       = new Vector2[vertices.Length];
        int[]     triangles = new int[width * depth * 6];

        float[,] heightMap = GenerateHeightMap();

        int vertIndex = 0;
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = heightMap[x, z] * maxHeight;
                vertices[vertIndex] = new Vector3(x, y, z);
                uvs[vertIndex]      = new Vector2((float)x / width, (float)z / depth);
                vertIndex++;
            }
        }

        // 삼각형(Triangle) 생성
        int triIndex = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int v00 = z * (width + 1) + x;
                int v10 = v00 + 1;
                int v01 = v00 + (width + 1);
                int v11 = v01 + 1;

                // 사각형을 두 개의 삼각형으로 분할
                triangles[triIndex++] = v00;
                triangles[triIndex++] = v01;
                triangles[triIndex++] = v10;

                triangles[triIndex++] = v10;
                triangles[triIndex++] = v01;
                triangles[triIndex++] = v11;
            }
        }

        mesh.vertices  = vertices;
        mesh.uv        = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 조명 계산을 위해 법선 재계산
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// 옥타브 Perlin Noise로 자연스러운 높이맵 생성
    /// </summary>
    float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[width + 1, depth + 1];

        // 시드 기반 오프셋 (같은 패턴 반복 방지)
        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-10000, 10000);
            float offsetZ = rng.Next(-10000, 10000);
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        float maxNoiseValue = float.MinValue;
        float minNoiseValue = float.MaxValue;

        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float amplitude  = 1f;
                float frequency  = 1f;
                float noiseValue = 0f;

                for (int oct = 0; oct < octaves; oct++)
                {
                    float sampleX = (x * noiseScale * frequency) + octaveOffsets[oct].x;
                    float sampleZ = (z * noiseScale * frequency) + octaveOffsets[oct].y;

                    noiseValue += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;

                    amplitude *= persistence;  // 진폭 감소
                    frequency *= lacunarity;   // 주파수 증가
                }

                heightMap[x, z] = noiseValue;

                if (noiseValue > maxNoiseValue) maxNoiseValue = noiseValue;
                if (noiseValue < minNoiseValue) minNoiseValue = noiseValue;
            }
        }

        // 0~1 범위로 정규화
        for (int z = 0; z <= depth; z++)
            for (int x = 0; x <= width; x++)
                heightMap[x, z] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, heightMap[x, z]);

        return heightMap;
    }

    // Inspector에서 버튼처럼 사용 가능 (Editor 모드에서 재생성)
    [ContextMenu("지형 재생성")]
    void RegenerateTerrain() => GenerateTerrain();
}