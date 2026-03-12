using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AutoFitCapsuleCollider : MonoBehaviour
{
    [ContextMenu("캡슐 콜라이더 자동 맞추기")]
    public void FitCapsuleToBody()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        // 팔(손) 제외하고 몸통 본만 찾기
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds totalBounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            totalBounds.Encapsulate(r.bounds);
        }

        // 캡슐 설정
        float height = totalBounds.size.y;           // 키
        float radius = Mathf.Max(
            totalBounds.size.x,
            totalBounds.size.z) / 2f * 0.5f;        // 팔 제외 → 절반만 사용

        col.direction = 1;                            // Y축 방향 (세로)
        col.height = height;
        col.radius = radius;
        col.center = transform.InverseTransformPoint(totalBounds.center);
    }
}