using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] public float _moveSpeed;
    [SerializeField] public float _runSpeed;
    
    [Header("회전 속도")]
    [SerializeField] private float _rotateSpeed;
    
    private Rigidbody _rigidbody;
    private PlayerController _controller;
    Vector3 _movement = Vector3.zero;

    private bool _prevIsMoving;
    private bool _prevRunning;
    private bool _isRunning;
    public bool IsMoving { get; private set; }

    public event Action<float> OnMove;
    public event Action<bool> OnRunChanged;
    public bool IsRunning => _isRunning;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        Rotate();
        Move();
    }

    private void Update()
    { 
        HandleMovement();
        bool now = Input.GetKey(KeyCode.LeftShift);
        if(now != _prevIsMoving)
        {
            _prevRunning = now;
            _isRunning = now;
            OnRunChanged?.Invoke(_prevRunning);
        }
        else
        {
            _isRunning = now;
        }
        
        OnMove?.Invoke(_movement.magnitude);
    }
    
    private void Rotate()
    {
        if (!IsMoving) return;
        
        transform.forward = _movement;
    }

    private void Move()
    {
        if (!IsMoving)return;
        
        float speed = _isRunning ? _runSpeed : _moveSpeed;
        
        _rigidbody.MovePosition(_rigidbody.position + _movement * (_moveSpeed * Time.fixedDeltaTime));
    }

    private void HandleMovement()
    {
        _prevIsMoving = IsMoving;
        
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.z = Input.GetAxisRaw("Vertical");
        
        _movement.Normalize();

        IsMoving = _movement != Vector3.zero;

        if (IsMoving != _prevIsMoving)
        {
            OnMove?.Invoke(_movement.magnitude);
        }
    }
}
