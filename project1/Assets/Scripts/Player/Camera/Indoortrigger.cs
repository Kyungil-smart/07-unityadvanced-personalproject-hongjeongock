using UnityEngine;

public class Indoortrigger : MonoBehaviour
{
   [SerializeField] private QuarterViewCamera quarterViewCamera;

   [SerializeField] private string playerTag = "Player";

   private void OnTriggerEnter(Collider other)
   {
       if (!other.CompareTag(playerTag)) return;

       Debug.Log("[IndoorTrigger] 플레이어 실내 진입 -> 탑뷰로 전환");
       quarterViewCamera.SetIndoor(true);
   }

   private void OnTriggerExit(Collider other)
   {
       if (!other.CompareTag(playerTag)) return;

       Debug.Log("[IndoorTrigger] 플레이어 실내 탈출 -> 쿼터뷰로 전환");
       quarterViewCamera.SetIndoor(false);
   }
}
