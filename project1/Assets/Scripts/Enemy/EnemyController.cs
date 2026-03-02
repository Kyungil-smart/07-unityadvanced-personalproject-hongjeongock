using System;
using UnityEngine;
using UnityEngine.UIElements;


// 좀비 1개체의 모든 동작을 관리하는 스크립트
// 플레이어 추적, 공격, 피격, 사망 등
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("타겟 대상")]
    [SerializeField] private Transform target;

    [Header("이동")]
    [SerializeField] private float _moveSpeed = 2.5f; // 이동 속도
    [SerializeField] private float _rotateSpeed = 720f; // 회전 속도
    [SerializeField] private float _stopDistance = 1.2f; // 타겟과의 최소 거리(거리보다 가까워지면 멈춤)

    [Header("전투")]
    [SerializeField] private float _maxHP = 50f; // 최대 체력
    [SerializeField] private float _attackDamage = 5f; // 공격력
    [SerializeField] private float _attackRange = 1.5f; // 공격 범위
    [SerializeField] private float _attackCooldown = 1.5f; // 공격 쿨타임

    private Rigidbody _rb;
    private float _currentHP;
    private float _nextAttackTime;
    private bool _isDie;

    public event System.Action OnDeath; // 사망 시 이벤트

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentHP = _maxHP;

        // 좀비기 넘어지지 않도록 회전 고정
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        if(target == null)
        {
            // 플레이어 태그로 타겟 자동 설정
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if(_isDie)return;
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        RotateToTarget();

        if(distance <= _attackRange)
        {
            StopMove();
            TryAttack();
        }
        else if (distance > _stopDistance)
        {
            MoveToTarget();
        }
        else
        {
            StopMove();
        }
    }

    private void MoveToTarget()
    {
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;
        dir = dir.normalized;

        Vector3 nextPosition = _rb.position + dir * _moveSpeed * Time.fixedDeltaTime;
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * _rotateSpeed * Time.fixedDeltaTime);
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime) return;

       _nextAttackTime = Time.time + _attackCooldown;

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_attackDamage, transform.position);
        }
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (_isDie) return;

        _currentHP -= damage;

        if (_currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if(_isDie) return;

        _isDie = true;

        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}
