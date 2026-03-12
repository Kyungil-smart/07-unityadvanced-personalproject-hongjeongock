using UnityEngine;

/// <summary>
/// 배틀로얄 스타일 섬 지형 생성기
/// - 원형 섬 모양 (가장자리로 갈수록 바다 아래로)
/// - Perlin Noise로 자연스러운 해안선 굴곡
/// - 버텍스 컬러: 모래/잔디/흙/바위
///
/// 사용법: 빈 GameObject에 추가, 머티리얼 연결 후 플레이
///         우클릭 → 섬 재생성으로 에디터에서도 미리보기 가능
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class IslandGenerator : MonoBehaviour
{
    [Header("섬 크기")]
    public int   resolution   = 256;   // 해상도 (높을수록 디테일)
    public float islandSize   = 500f;  // 섬 전체 크기
    public float islandRadius = 200f;  // 섬 반지름

    [Header("높이 설정")]
    public float maxHeight  = 30f;   // 섬 최대 높이
    public float waterLevel = 0f;    // 수면 높이

    [Header("Perlin Noise")]
    public float noiseScale  = 0.006f;
    public int   octaves     = 6;
    public float persistence = 0.5f;
    public float lacunarity  = 2.0f;

    [Header("섬 모양")]
    [Range(1f, 5f)]
    public float edgeSharpness = 2.5f;   // 클수록 해안선이 가파름
    public float coastNoise    = 0.4f;   // 해안선 불규칙도 (0=완전한 원)

    [Header("머티리얼")]
    public Material terrainMaterial;     // Vertex Color 지원 머티리얼 권장

    [Header("랜덤 시드")]
    public int  seed          = 1234;
    public bool useRandomSeed = true;

    void Start() => GenerateIsland();

    public void GenerateIsland()
    {
        if (useRandomSeed) seed = Random.Range(0, 99999);

        var mf = GetComponent<MeshFilter>();
        var mr = GetComponent<MeshRenderer>();
        var mc = GetComponent<MeshCollider>();

        if (mr != null && terrainMaterial != null)
            mr.material = terrainMaterial;

        var mesh = BuildMesh();
        mf.sharedMesh = mesh;
        mc.sharedMesh = mesh;

        Debug.Log($"[IslandGenerator] 생성 완료 | Seed:{seed} | 크기:{islandSize} | 반지름:{islandRadius}");
    }

    Mesh BuildMesh()
    {
        var mesh = new Mesh();
        mesh.name        = "ProceduralIsland";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int verts = (resolution + 1) * (resolution + 1);
        var vertices  = new Vector3[verts];
        var uvs       = new Vector2[verts];
        var colors    = new Color[verts];
        var triangles = new int[resolution * resolution * 6];

        // 시드 기반 노이즈 오프셋
        var rng = new System.Random(seed);
        var offsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
            offsets[i] = new Vector2(rng.Next(-9999, 9999), rng.Next(-9999, 9999));

        // 해안선 굴곡용 오프셋 (별도 시드)
        var coastOffsets = new Vector2[3];
        for (int i = 0; i < 3; i++)
            coastOffsets[i] = new Vector2(rng.Next(-9999, 9999), rng.Next(-9999, 9999));

        float half = islandSize * 0.5f;

        // ── 정점 생성 ──
        int vi = 0;
        for (int z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float nx = (float)x / resolution;
                float nz = (float)z / resolution;

                // 월드 좌표 (중심=0,0)
                float wx = (nx - 0.5f) * islandSize;
                float wz = (nz - 0.5f) * islandSize;

                // 해안선에 노이즈 추가 → 울퉁불퉁한 섬 모양
                float coastWarp = 0f;
                for (int i = 0; i < 3; i++)
                {
                    float cs = 0.003f * (i + 1);
                    coastWarp += Mathf.PerlinNoise(
                        nx * islandSize * cs + coastOffsets[i].x,
                        nz * islandSize * cs + coastOffsets[i].y) * coastNoise / (i + 1);
                }

                // 중심에서 거리 (노이즈로 뒤틀림)
                float dist = Mathf.Sqrt(wx * wx + wz * wz) / (islandRadius * (1f + coastWarp - coastNoise * 0.5f));

                // 섬 마스크 (1=중앙, 0=바다)
                float mask = Mathf.Clamp01(1f - Mathf.Pow(dist, edgeSharpness));
                // SmoothStep으로 더 부드럽게
                mask = mask * mask * (3f - 2f * mask);

                // Perlin Noise 높이
                float noise = 0f;
                float amp = 1f, freq = 1f, maxAmp = 0f;
                for (int o = 0; o < octaves; o++)
                {
                    float sx = nx * islandSize * noiseScale * freq + offsets[o].x;
                    float sz = nz * islandSize * noiseScale * freq + offsets[o].y;
                    noise  += Mathf.PerlinNoise(sx, sz) * amp;
                    maxAmp += amp;
                    amp    *= persistence;
                    freq   *= lacunarity;
                }
                noise /= maxAmp; // 0~1 정규화

                // 중앙은 약간 평탄하게 (건물 배치 공간 확보)
                float centerFlat = Mathf.Clamp01(1f - dist * 1.5f);
                noise = Mathf.Lerp(noise, 0.4f + noise * 0.3f, centerFlat * 0.4f);

                float height = noise * mask * maxHeight;

                // 수면 아래는 더 깎기
                if (dist > 0.95f) height = Mathf.Min(height, waterLevel - 1f);

                vertices[vi] = new Vector3(wx, height, wz);
                uvs[vi]      = new Vector2(nx, nz);
                colors[vi]   = GetVertexColor(height, mask, dist);
                vi++;
            }
        }

        // ── 삼각형 생성 ──
        int ti = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int v00 = z * (resolution + 1) + x;
                int v10 = v00 + 1;
                int v01 = v00 + (resolution + 1);
                int v11 = v01 + 1;

                triangles[ti++] = v00; triangles[ti++] = v01; triangles[ti++] = v10;
                triangles[ti++] = v10; triangles[ti++] = v01; triangles[ti++] = v11;
            }
        }

        mesh.vertices  = vertices;
        mesh.uv        = uvs;
        mesh.colors    = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    Color GetVertexColor(float height, float mask, float dist)
    {
        // 해변 (수면 근처 or 가장자리)
        if (height < maxHeight * 0.08f || dist > 0.80f)
            return new Color(0.80f, 0.73f, 0.52f); // 모래

        // 낮은 잔디
        if (height < maxHeight * 0.30f)
            return Color.Lerp(
                new Color(0.80f, 0.73f, 0.52f),  // 모래
                new Color(0.48f, 0.65f, 0.32f),  // 연한 잔디
                (height / (maxHeight * 0.30f)));

        // 잔디
        if (height < maxHeight * 0.60f)
            return new Color(0.35f, 0.55f, 0.25f); // 잔디

        // 언덕 (흙)
        if (height < maxHeight * 0.80f)
            return new Color(0.42f, 0.37f, 0.28f); // 흙

        // 정상 (바위)
        return new Color(0.55f, 0.52f, 0.48f); // 바위
    }

    [ContextMenu("섬 재생성")] void Regenerate() => GenerateIsland();
}