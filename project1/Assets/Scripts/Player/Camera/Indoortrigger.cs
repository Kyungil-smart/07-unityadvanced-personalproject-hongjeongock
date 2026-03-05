using UnityEngine;

public class Indoortrigger : MonoBehaviour
{
   [SerializeField] private QuarterViewCamera quarterViewCamera;

   [SerializeField] private string playerTag = "Player";

   private void Awake()
   {
       if (quarterViewCamera == null)
           quarterViewCamera = FindFirstObjectByType<QuarterViewCamera>();
   }
   private void OnTriggerEnter(Collider other)
   {
       if (!other.CompareTag(playerTag)) return;
       
       if (quarterViewCamera == null)
       {
           Debug.LogError("[IndoorTrigger] quarterViewCamera가 null 입니다. 인스펙터 연결 또는 Awake 자동탐색 확인");
           return;
       }

       Debug.Log("[IndoorTrigger] 플레이어 실내 진입 -> 탑뷰로 전환");
       quarterViewCamera.SetIndoor(true);
   }

   private void OnTriggerExit(Collider other)
   {
       if (!other.CompareTag(playerTag)) return;
       
       if (quarterViewCamera == null)
       {
           Debug.LogError("[IndoorTrigger] quarterViewCamera가 null 입니다. 인스펙터 연결 또는 Awake 자동탐색 확인");
           return;
       }

       Debug.Log("[IndoorTrigger] 플레이어 실내 탈출 -> 쿼터뷰로 전환");
       quarterViewCamera.SetIndoor(false);
   }
}
