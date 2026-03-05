using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("플레이어 최대 체력")]
    [SerializeField] public float _playerMaxHp;
    [Header("플레이어 현재 체력")]
    [SerializeField] public float _playerCurrentHp;
    
    [Header("플레이어 공격력")]
    [SerializeField] public float _playerDamage;
    
    [Header("플레이어 공격 쿨타임")]
    [SerializeField] public float _playerATKTime;
    
    [Header("플레이어 공격 범위")]
    [SerializeField] public float _playerAttackRange = 2f;
    
    [SerializeField] private GameOverUI _gameOverUI;

    private float _nextAttackTime;

    public event Action OnDeath;
    public event Action OnAttack;
    public event Action<float, float> OnHPChanged;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        ResetHP();
    }

    private void Update()
    {
        if (IsDead) return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("공격!");
            Attack();
        }
    }

    public void ResetHP()
    {
        _playerCurrentHp = _playerMaxHp;
        IsDead = false;
        OnHPChanged?.Invoke(_playerCurrentHp,  _playerMaxHp);
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        if (IsDead) return;

        _playerCurrentHp -= damage;
        _playerCurrentHp = Mathf.Max(0f, _playerCurrentHp);

        OnHPChanged?.Invoke(_playerCurrentHp,  _playerMaxHp);

        if (_playerCurrentHp <= 0f)
            Die();
    }

    public void Attack()
    {
        if (IsDead) return;
        if (Time.time < _nextAttackTime) return;
    
        _nextAttackTime = Time.time + _playerATKTime;
        OnAttack?.Invoke();

        Collider[] hits = Physics.OverlapSphere(transform.position, _playerAttackRange);
        Debug.Log($"공격 범위 내 오브젝트 수: {hits.Length}");

        foreach (var col in hits)
        {
            if (col.gameObject == gameObject) continue;
            Debug.Log($"감지된 오브젝트: {col.gameObject.name}");
        
            if (col.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_playerDamage, col.transform.position);
            }
        }
    }
    
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        
        float prevHp = _playerCurrentHp;
        
        _playerCurrentHp = Mathf.Min(_playerCurrentHp + amount, _playerMaxHp);

        if (!Mathf.Approximately(prevHp, _playerCurrentHp))
        {
            OnHPChanged?.Invoke(_playerCurrentHp, _playerMaxHp);
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        if (_gameOverUI != null)
        {
            _gameOverUI.Show();
        }
        else
        {
            Debug.LogError("[PlayerController] GameOverUI가 연결되지 않았습니다! PlayerController 인스펙터에 할당하세요.");
        }
        OnDeath?.Invoke();
    }
}