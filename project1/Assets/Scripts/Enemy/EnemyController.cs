using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("타겟 대상")]
    [SerializeField] private Transform target;

    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2.5f; // 이동 속도
    [SerializeField] private float _runSpeed = 4.5f; // 달리기 속도 플레이어 보다 좀... (많이)느리게..
    [SerializeField] private float _rotateSpeed = 720f; // 회전 속도
    [SerializeField] private float _stopDistance = 1.2f; // 타겟과의 최소 거리(거리보다 가까워지면 멈춤)

    [Header("감지")]
    [SerializeField] private float _detctionRange = 20f;
    [SerializeField] private float _runRange = 8f;

    [Header("전투")]
    [SerializeField] private float _maxHP = 50f; // 최대 체력
    [SerializeField] private float _attackDamage = 5f; // 공격력
    [SerializeField] private float _attackRange = 1.5f; // 공격 범위
    [SerializeField] private float _attackCooldown = 1.5f; // 공격 쿨타임
    [SerializeField] private int xpReward = 3;

    private Rigidbody _rb;
    private Animator _animator;
    private float _currentHP;
    private float _nextAttackTime;
    private bool _isDie;
    private PlayerLevelSystem _playerLevelSystem;
    
    [Header("사운드")]
    [SerializeField] private AudioSource _idleAudioSource;
    [SerializeField] private AudioSource _attackAudioSource;
    [SerializeField] private AudioClip _idleClip;
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _deathClip;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsAttack = Animator.StringToHash("isAttack");
    private static readonly int HashIsDead = Animator.StringToHash("isDead");
    
    public event System.Action OnDeath;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _currentHP = _maxHP;
        
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        _idleAudioSource.spatialBlend = 2f; 
        _idleAudioSource.maxDistance = 15f;     
        _idleAudioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    private void Start()
    {
        if(target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        _playerLevelSystem = FindObjectOfType<PlayerLevelSystem>();
        _idleAudioSource.clip = _idleClip;
        _idleAudioSource.loop = true;
        _idleAudioSource.Play();
    }

    private void FixedUpdate()
    {
        if(_isDie) return;
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        RotateToTarget();

        if (distance <= _attackRange)
        {
            StopMove();
            TryAttack();
            SetAnimatorSpeed(0f);
        }
        else if (distance <= _detctionRange && distance > _stopDistance)
        {
            bool shouldRun = distance <= _runRange;
            float speed = shouldRun ? _runSpeed : _moveSpeed;

            MoveToTarget(speed);
            SetAnimatorSpeed(shouldRun ? 1f : 0.3f);
            SetAttack(false);
        }
        else
        {
            StopMove();
            SetAnimatorSpeed(0f);
            SetAttack(false);
        }
    }

    private void MoveToTarget(float speed)
    {
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;
        dir = dir.normalized;

        Vector3 nextPosition = _rb.position + dir * speed * Time.fixedDeltaTime;
        _rb.MovePosition(nextPosition);
    }

    private void StopMove()
    {
        _rb.linearVelocity = Vector3.zero;
    }

    private void RotateToTarget()
    {
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;
        
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.fixedDeltaTime);
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime)
        {
            SetAttack(false);
            return;
        }

       _nextAttackTime = Time.time + _attackCooldown;
       
       SetAttack(true);

       if (target.TryGetComponent<IDamageable>(out var damageable))
       {
           damageable.TakeDamage(_attackDamage, transform.position);
           _attackAudioSource.PlayOneShot(_attackClip);
       }
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (_isDie) return;

        _currentHP -= damage;

        if (_currentHP <= 0f)
        {
            Debug.Log("좀비 사망!");
            Die();
        }
    }

    private void Die()
    {
        if(_isDie) return;

        _isDie = true;

        if (_playerLevelSystem != null)
        {
            _playerLevelSystem.AddXP(xpReward);
        }

        _idleAudioSource.Stop();
        
        _attackAudioSource.PlayOneShot(_deathClip);
            
        GetComponent<EnemyLootDropper>()?.DropLoot();

        OnDeath?.Invoke();

        Destroy(gameObject, 0.1f);
    }

    private void SetAnimatorSpeed(float speed)
    {
        _animator?.SetFloat(HashSpeed, speed);
    }

    private void SetAttack(bool value)
    {
        _animator?.SetBool(HashIsAttack,  value);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _moveSpeed);
        
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _runRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
#endif
}
