using System;
using UnityEngine;

public class QuarterViewCamera : MonoBehaviour
{
    [Header("참조 설정")] 
    [SerializeField] private Transform target;
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform cam;
    
    [Header("플레이어 추적")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private float follwSmooth;

    [Header("줌 설정")]
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float zoomSmooth;

    [Header("카메라 충돌 방지")] 
    [SerializeField] private float collisionRadius;
    [SerializeField] private float collisionBuffer;
    [SerializeField] private LayerMask collisionMask;
    
    private float _targetDistance;
    private float _currentDistance;

    private void Reset()
    {
        collisionMask = ~LayerMask.GetMask("Player");
    }

    private void Awake()
    {
        if (cam == null)
        {
            _currentDistance =Mathf.Abs(cam.localPosition.z);
            _targetDistance = _currentDistance;
        }
    }

    private void LateUpdate()
    {
        if(!target || !pivot || !cam)return;
        
        Vector3 desiredPos = target.position + targetOffset;
        transform.position = Vector3.Lerp (
            transform.position, desiredPos, 1f - Mathf.Exp(-follwSmooth * Time.deltaTime)
            );
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            _targetDistance -= scroll * zoomSpeed;
            _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);
        }
        _currentDistance = Mathf.Lerp(
            _currentDistance, _targetDistance, 1f - Mathf.Exp(-zoomSmooth * Time.deltaTime)
            );
        
        Vector3 desiredLocal = new Vector3(0f, 0f, -_currentDistance);
        Vector3 pivotWorld = pivot.position;
        Vector3 desiredWorld = pivot.TransformPoint(desiredLocal);
        
        Vector3 dir = desiredWorld - pivot.position;
        float dist = dir.magnitude;
        if(dist < 0.0001f) dist = 0.0001f;
        dir /= dist;

        float correctedDist = dist;

        if (Physics.SphereCast(
                pivot.position,
                collisionRadius,
                dir,
                out RaycastHit hit,
                dist,
                collisionMask,
                QueryTriggerInteraction.Ignore))
        {
            correctedDist = Mathf.Max(0.2f, hit.distance - collisionBuffer);
        }
        
        cam.localPosition = new Vector3(0f, 0f, -correctedDist);
        
    }
}
