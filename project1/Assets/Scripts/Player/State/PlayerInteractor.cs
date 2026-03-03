using System.Collections;
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
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("플레이어 컨트롤러 참조")]
    [SerializeField] private PlayerController playerController;

    private IInteractable _current;
    private IInteractable _target;

    // 채집 쿨다운 (빠른 연타 방지)
    private float _gatherCooldown = 0.5f;
    private float _lastGatherTime;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerTransform == null) playerTransform = this.transform;
    }

    private void Update()
    {
        DetectInteractable();

        if (inputGate != null && inputGate.IsLocked)
        {
            _target = null;
            return;
        }

        FindTarget();
        
        if (_target != null && !(_target is GatherableResource) && Input.GetKeyDown(interactKey))
        {
            _target.Interact(gameObject);
        }
        
        if (_target is GatherableResource gatherable && Input.GetMouseButtonDown(0))
        {
            if (Time.time - _lastGatherTime >= _gatherCooldown)
            {
                _lastGatherTime = Time.time;
                
                //if (playerController != null)
               //     playerController.TriggerAttack();

                gatherable.Interact(gameObject);
            }
        }

        Debug.DrawRay(playerTransform.position + Vector3.up * 1f, playerTransform.forward * interactDistance, Color.red);
    }

    private void FindTarget()
    {
        _target = null;
        var ray = new Ray(playerTransform.position + Vector3.up * 1f, playerTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            _target = hit.collider.GetComponentInParent<IInteractable>();
        }
    }

    private void DetectInteractable()
    {
        if (_current != null) _current.LockOn(false);
        _current = null;

        Ray ray = new Ray(playerTransform.position + Vector3.up * 1f, playerTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            _current = hit.collider.GetComponentInParent<IInteractable>();
            if (_current != null) _current.LockOn(true);
        }
    }
}