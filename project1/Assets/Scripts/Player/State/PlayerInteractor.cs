using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("필수 참조")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputGate inputGate;

    [SerializeField] private Transform playerTransform;

    [Header("레이캐스트 설정")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask = ~0;

    [Header("상호작용 키")]
    [SerializeField] private KeyCode InteractKey = KeyCode.E;

    private IInteractable _current;
    private IInteractable _target;

    private void Awake()
    {
        if(playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerTransform == null)
        {
            playerTransform = transform;
        }
    }

    private void Update()
    {
        DetectInteractable();

        if(_current != null)
        {
            _current.LockOn(true);
        }
        if (inputGate != null && inputGate.IsLocked)
        {
            _target = null;
            return;
        }

        FindTarget();

        if (_target != null && Input.GetKeyDown(KeyCode.E))
        {
            _target.Interact(gameObject);
        }

        // 디버그용 레이 방향 확인용
        Debug.DrawRay(
            playerTransform.position + Vector3.up * 1f,
            playerTransform.forward * interactDistance,
            Color.red
        );
    }

        private void FindTarget()
    {
        _target = null;

        Vector3 rayOrigin = playerTransform.position + Vector3.up * 1f;
        Vector3 rayDirection = playerTransform.forward;

        var ray = new Ray(rayOrigin, rayDirection);

        if(Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            Debug.Log("Hit 이름 :" + hit.collider.name);
            Debug.Log("Hit 레이어 :" + LayerMask.LayerToName(hit.collider.gameObject.layer));

            _target = hit.collider.GetComponentInParent<IInteractable>();
            Debug.Log("Hit 인터렉터블 존재 : " + (_target != null));
        }

        else
        {
            Debug.Log("아무것도 없음");
        }
    }

    private void DetectInteractable()
    {
        if(_current != null)
        {
            _current.LockOn(false);
        }

        _current = null;

        if(playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            _current = hit.collider.GetComponentInParent<IInteractable>();

            if (_current != null)
            {
                _current.LockOn(true);
            }
        }
    }
}