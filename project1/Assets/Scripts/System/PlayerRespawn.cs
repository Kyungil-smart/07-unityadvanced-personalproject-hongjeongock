using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("리스폰 위치")]
    [SerializeField] private Transform respawnPoint;

    [Header("플레이어 리지드바디")]
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("플레이어 컨트롤러")]
    [SerializeField] private PlayerController playerController;

    public void RespawnToSafePoint()
    {
        Debug.Log("리스폰 함수 호출됨");

        if (respawnPoint == null)
        {
            Debug.LogWarning("리스폰 위치가 연결되지 않았습니다.");
            return;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.position = respawnPoint.position;
            playerRigidbody.rotation = respawnPoint.rotation;
        }
        else
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        Debug.Log("리스폰 완료: " + respawnPoint.position);
    }
}