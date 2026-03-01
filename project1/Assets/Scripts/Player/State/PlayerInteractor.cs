using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("의존성")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInputGate inputGate;

    [Header("레이캐스트 설정")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;

    private IInteractable _target;

    private void Update()
    {
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
    }

    private void FindTarget()
    {
        _target = null;

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            _target = hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}