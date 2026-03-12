using UnityEngine;

public class ZonePlacer : MonoBehaviour
{
    [Header("배치할 프리팹")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;

    [Header("배치 설정")]
    public int treeCount = 20;
    public int rockCount = 10;
    public float yOffset = 0f;

    [Header("범위 설정")]
    public Vector3 zoneSize = new Vector3(50f, 0f, 50f);

    [Header("레이어 설정")]
    public LayerMask groundLayer; // ← Inspector에서 Ground 레이어 선택

    [ContextMenu("오브젝트 생성")]
    public void PlaceObjects()
    {
        PlaceGroup(treePrefabs, treeCount, "Trees");
        PlaceGroup(rockPrefabs, rockCount, "Rocks");
    }

    [ContextMenu("생성한 오브젝트 삭제")]
    public void ClearObjects()
    {
        Transform treesParent = transform.Find("Trees");
        Transform rocksParent = transform.Find("Rocks");
        if (treesParent) DestroyImmediate(treesParent.gameObject);
        if (rocksParent) DestroyImmediate(rocksParent.gameObject);
    }

    void PlaceGroup(GameObject[] prefabs, int count, string groupName)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        GameObject parent = new GameObject(groupName);
        parent.transform.SetParent(transform);

        Vector3 center = transform.position;

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(center.x - zoneSize.x / 2f, center.x + zoneSize.x / 2f);
            float z = Random.Range(center.z - zoneSize.z / 2f, center.z + zoneSize.z / 2f);
            float y = GetGroundHeight(x, z);

            // 지면 못 찾으면 스킵
            if (y == float.MinValue) continue;

            Vector3 pos = new Vector3(x, y + yOffset, z);
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject obj = Instantiate(prefab, pos, rot, parent.transform);

            float scale = Random.Range(0.8f, 1.3f);
            obj.transform.localScale *= scale;
        }
    }

    float GetGroundHeight(float x, float z)
    {
        Ray ray = new Ray(new Vector3(x, 500f, z), Vector3.down);

        // groundLayer만 감지! 나무/돌 레이어는 무시
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            return hit.point.y;
        }

        return float.MinValue; // 지면 못 찾으면 MinValue 반환
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(zoneSize.x, 1f, zoneSize.z));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(zoneSize.x, 1f, zoneSize.z));
    }
}