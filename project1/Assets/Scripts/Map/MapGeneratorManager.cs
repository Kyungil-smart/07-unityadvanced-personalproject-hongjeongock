using UnityEngine;

/// <summary>
/// 지형 + 도시 통합 맵 생성 매니저
/// 사용법: 빈 GameObject에 추가, TerrainGenerator와 CityGenerator를 자동 연결
/// 이 스크립트 하나로 전체 맵을 한 번에 생성/재생성 가능
/// </summary>
public class MapGeneratorManager : MonoBehaviour
{
    [Header("생성 순서 설정")]
    public bool generateTerrainFirst = true;    // 지형 먼저 생성 후 도시 배치
    public float cityGenerationDelay = 0.5f;    // 지형 생성 후 도시 생성 딜레이(초)

    [Header("컴포넌트 참조")]
    public TerrainGenerator terrainGenerator;
    public CityGenerator     cityGenerator;

    [Header("공통 시드 사용")]
    public bool useSharedSeed = true;
    public int  sharedSeed    = 1234;

    void Start()
    {
        // 같은 GameObject에서 자동 참조
        if (terrainGenerator == null)
            terrainGenerator = GetComponentInChildren<TerrainGenerator>();
        if (cityGenerator == null)
            cityGenerator = GetComponentInChildren<CityGenerator>();

        GenerateFullMap();
    }

    public void GenerateFullMap()
    {
        Debug.Log("[MapManager] 전체 맵 생성 시작...");

        // 공통 시드 적용
        if (useSharedSeed)
        {
            if (terrainGenerator != null)
            {
                terrainGenerator.seed = sharedSeed;
                terrainGenerator.useRandomSeed = false;
            }
            if (cityGenerator != null)
            {
                cityGenerator.seed = sharedSeed + 1; // 시드를 약간 다르게 해서 다양성 확보
                cityGenerator.useRandomSeed = false;
            }
        }

        if (generateTerrainFirst)
        {
            terrainGenerator?.GenerateTerrain();
            // 지형 Mesh Collider가 적용된 후 도시 배치
            Invoke(nameof(GenerateCityDelayed), cityGenerationDelay);
        }
        else
        {
            terrainGenerator?.GenerateTerrain();
            cityGenerator?.GenerateCity();
        }
    }

    void GenerateCityDelayed()
    {
        cityGenerator?.GenerateCity();
        Debug.Log("[MapManager] 전체 맵 생성 완료!");
    }

    [ContextMenu("전체 맵 재생성")]
    void RegenerateAll()
    {
        sharedSeed = Random.Range(0, 99999);
        GenerateFullMap();
    }

    // 키보드 R키로 런타임 중 재생성
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[MapManager] R키 입력 - 맵 재생성");
            RegenerateAll();
        }
    }
}
