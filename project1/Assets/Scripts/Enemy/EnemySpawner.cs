using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject _enemyPrefab; // 스폰할 적 프리팹
    [SerializeField] private float _spawnInterval = 3f; // 스폰 간격
    [SerializeField] private int _maxEnemies = 20; // 최대 적 수

    [Header("스폰 범위")]
    [SerializeField] private float _spawnRadius = 25f;

    [Header("베이스(집) 보호 구역")]
    [SerializeField] private Transform _baseCenter; // 베이스 중심
    [SerializeField] private float _baseSafeRadius = 8f; // 베이스 보호 구역(이 구역 안에는 스폰x)

    private float _nextSpawnTime;
    private int _currentAlive;

    private void Update()
    {
        if(Time.time < _nextSpawnTime) return;
        if(_currentAlive >= _maxEnemies) return;

        TrySpawn();

        _nextSpawnTime = Time.time + _spawnInterval;
    }

    private void TrySpawn()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;

        Vector3 spawnPos = new Vector3(randomCircle.x, 0f, randomCircle.y);

        if(_baseCenter != null)
        {
           float distanceToBase = Vector3.Distance(spawnPos, _baseCenter.position);

              if (distanceToBase < _baseSafeRadius)
                return;
        }

        GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
        _currentAlive++;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);

        if (_baseCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_baseCenter.position, _baseSafeRadius);
        }
    }
    #endif
}
