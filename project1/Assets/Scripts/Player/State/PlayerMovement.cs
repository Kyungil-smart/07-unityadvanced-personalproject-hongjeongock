using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] public float _moveSpeed;
    [SerializeField] public float _runSpeed;
    
    [Header("회전 속도")]
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _mouseRotateSpeed = 200f;
    
    private Rigidbody _rigidbody;
    Vector3 _movement = Vector3.zero;

    private bool _prevIsMoving;
    private bool _prevRunning;
    private bool _isRunning;
    public bool IsMoving { get; private set; }
    public bool IsRunning => _isRunning;

    public event Action<float> OnMove;
    public event Action<bool> OnRunChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position + Vector3.up, _movement * 3f, Color.red);
        Rotate();
        Move();
    }

    private void Update()
    { 
        HandleMovement();
        bool nowRunning = Input.GetKey(KeyCode.LeftShift);
        if(nowRunning != _prevRunning)
        {
            _prevRunning = nowRunning;
            _isRunning = nowRunning;
            OnRunChanged?.Invoke(_isRunning);
        }
        else
        {
            _isRunning = nowRunning;
        }
        
        OnMove?.Invoke(_movement.magnitude);
    }
    
    private void Rotate()
    {
        if(!IsMoving) return;

        Quaternion targetRot = Quaternion.LookRotation(_movement);
        Quaternion newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, _rotateSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(newRot);

        float mouseX = Input.GetAxis("Mouse X");

        float angle = mouseX * _mouseRotateSpeed * Time.fixedDeltaTime;

        Quaternion deltaRot = Quaternion.Euler(0f, angle, 0f);
         _rigidbody.MoveRotation(_rigidbody.rotation * deltaRot);
    }

    private void Move()
    {
        if (!IsMoving) return;

        float speed = _isRunning ? _runSpeed : _moveSpeed;

        Vector3 nextPos = _rigidbody.position + _movement * (speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(nextPos);
    }

    private void HandleMovement()
    {
        _prevIsMoving = IsMoving;
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        if (input.sqrMagnitude < 0.0001f)
        {
            _movement = Vector3.zero;
            IsMoving = false;
            
            if(IsMoving != _prevIsMoving)
                OnMove?.Invoke(0f);

            return;
        }

        Transform camT = Camera.main.transform;
        Vector3 camForward = camT.forward;
        Vector3 camRight = camT.right;
        
        camForward.y = 0f;
        camRight.y = 0f;
        
        camForward.Normalize();
        camRight.Normalize();
        
        _movement = (camForward * z + camRight * x).normalized;
        
        IsMoving = true;
        
        if(IsMoving != _prevIsMoving)
            OnMove?.Invoke(_movement.magnitude);
    }
}
