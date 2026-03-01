using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private readonly int _moveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int _isAtk = Animator.StringToHash("isAtk");
    private readonly int _isDie = Animator.StringToHash("isDead");
    private readonly int _isRun = Animator.StringToHash("isRun");
    
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerController _playerController;

    private void Awake() => Init();
    private void OnEnable() => SubscribeEvents();
    private void OnDisable() => UnsubscribeEvents();

    private void Update()
    {
        _animator.SetBool(_isRun, _playerMovement.IsRunning);
    }

    private void Init()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerController = GetComponent<PlayerController>();
    }

    private void SubscribeEvents()
    {
        _playerMovement.OnMove += OnAnimMove;
        _playerController.OnDeath += OnAnimDie;
        _playerController.OnAttack += OnAnimAtk;
    }

    private void UnsubscribeEvents()
    {
        _playerMovement.OnMove -= OnAnimMove;
        _playerController.OnDeath -= OnAnimDie;
        _playerController.OnAttack -= OnAnimAtk;
    }
    private void OnAnimMove(float movement) => _animator.SetFloat(_moveSpeed, movement);
    private void OnAnimAtk() => _animator.SetTrigger(_isAtk);
    private void OnAnimDie() => _animator.SetTrigger(_isDie);
    
    private void OnAnimRun() => _animator.SetTrigger(_isRun);
}
