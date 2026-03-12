using UnityEngine;

/// <summary>
/// 인게임 씬 시작 시 CharacterData를 읽어 플레이어를 조립하는 스폰너
/// 
/// [Inspector 연결 필요 항목]
/// - playerBasePrefab  : 기본 플레이어 프리팹 (이동/카메라 컴포넌트 포함)
/// - bodyPrefabs       : 몸 외형 프리팹 배열 (커스터마이징과 동일 순서)
/// - hairPrefabs       : 헤어 프리팹 배열
/// - outfitPrefabs     : 의상 프리팹 배열
/// - spawnPoint        : 플레이어 스폰 위치
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [Header("플레이어 베이스 프리팹")]
    [SerializeField] private GameObject playerBasePrefab;

    [Header("외형 프리팹 배열 (CustomizationController와 동일 순서)")]
    [SerializeField] private GameObject[] bodyPrefabs;
    [SerializeField] private GameObject[] hairPrefabs;
    [SerializeField] private GameObject[] outfitPrefabs;

    [Header("스폰 위치")]
    [SerializeField] private Transform spawnPoint;

    [Header("외형을 붙일 자식 Transform 이름")]
    [SerializeField] private string bodySocketName  = "BodySocket";
    [SerializeField] private string hairSocketName  = "HairSocket";
    [SerializeField] private string outfitSocketName = "OutfitSocket";

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerBasePrefab == null)
        {
            Debug.LogError("[PlayerSpawner] playerBasePrefab이 연결되지 않았습니다.");
            return;
        }

        // 1. 베이스 프리팹 생성
        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        GameObject player = Instantiate(playerBasePrefab, pos, rot);

        // 2. CharacterData 읽기
        CharacterData data = GameManager.Instance?.characterData;
        if (data == null)
        {
            Debug.LogWarning("[PlayerSpawner] CharacterData 없음 - 기본 외형으로 생성합니다.");
            return;
        }

        // 3. 외형 파츠 장착
        AttachPart(player, bodyPrefabs,   data.selectedBodyIndex,   bodySocketName);
        AttachPart(player, hairPrefabs,   data.selectedHairIndex,   hairSocketName);
        AttachPart(player, outfitPrefabs, data.selectedOutfitIndex, outfitSocketName);

        // 4. 색상 적용 (선택사항 - 렌더러 머티리얼 조정)
        ApplyColors(player, data);

        Debug.Log($"[PlayerSpawner] 플레이어 '{data.playerName}' 스폰 완료");
    }

    /// <summary>소켓 Transform에 외형 파츠 인스턴스를 붙임</summary>
    private void AttachPart(GameObject player, GameObject[] prefabs, int index, string socketName)
    {
        if (prefabs == null || prefabs.Length == 0) return;
        index = Mathf.Clamp(index, 0, prefabs.Length - 1);
        if (prefabs[index] == null) return;

        Transform socket = player.transform.Find(socketName);
        if (socket == null)
        {
            // 소켓이 없으면 플레이어 바로 아래에 붙임
            Debug.LogWarning($"[PlayerSpawner] '{socketName}' 소켓을 찾지 못했습니다. 루트에 붙입니다.");
            socket = player.transform;
        }

        Instantiate(prefabs[index], socket);
    }

    /// <summary>머티리얼 색상 적용 (프로젝트에 맞게 수정 필요)</summary>
    private void ApplyColors(GameObject player, CharacterData data)
    {
        // 예시: 헤어 소켓 하위 렌더러에 색상 적용
        Transform hairSocket = player.transform.Find(hairSocketName);
        if (hairSocket != null)
        {
            foreach (var renderer in hairSocket.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = data.hairColor;
            }
        }

        Transform bodySocket = player.transform.Find(bodySocketName);
        if (bodySocket != null)
        {
            foreach (var renderer in bodySocket.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = data.skinColor;
            }
        }
    }
}
