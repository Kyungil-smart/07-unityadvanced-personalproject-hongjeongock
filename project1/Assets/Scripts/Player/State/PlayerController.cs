using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 최대 체력")]
    [SerializeField] public float _playerMaxHp;
    [Header("플레이어 현재 체력")]
    [SerializeField] public float _playerCurrentHp;
    
    [Header("플레이어 공격력")]
    [SerializeField] public float _playerDamage;
    [Header("플레이어 공격 쿨타임")]
    [SerializeField] public float _playerATKTime;

    private float _nextAttackTime;

    public event Action OnDeath;
    public event Action OnAttack;
    public bool IsDead { get; private set; }

    private void Update()
    {
        if(IsDead)return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("공격!");
            Attack();
        }
    }
    private void Awake()
    {
        ResetHP();
    }

    public void ResetHP()
    {
        _playerCurrentHp = _playerMaxHp;
        IsDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        _playerCurrentHp -= damage;

        if (_playerCurrentHp <= 0f)
        {
            _playerCurrentHp = 0f;
            Die();
        }
    }

    public void Attack()
    {
        if(IsDead) return;
        if(Time.time <_nextAttackTime)return;
        
        _nextAttackTime = Time.time + _playerATKTime;
        OnAttack?.Invoke();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke();
    }
}
