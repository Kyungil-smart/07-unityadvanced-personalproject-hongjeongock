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

    public event Action OnDeath;
    public bool IsDead { get; private set; }

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

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath?.Invoke();
    }
}
