using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableResource : MonoBehaviour, IInteractable
{
    [Header("기본 설정")]
    [SerializeField] private string resourceName = "나무";
    [SerializeField] private int maxHP = 5;

    [Header("드랍 설정")] [SerializeField] private LootTableDefinition lootTable;
    
    [Header("이펙트/파티클")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ParticleSystem destroyParticle;

    [Header("사운드")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroySound;
    
    [Header("재생성 설정")]
    [SerializeField] private float respawnTime = 30f;
    [SerializeField] private GameObject visualObject;
    
    [Header("오브젝트 체력 바")]
    [SerializeField] private GameObject hpBarObject;
    [SerializeField] private UnityEngine.UI.Slider hpSlider;

    public string Prompt => _isDestory ? "" : $"{resourceName} 채집";
    
    private int _currentHP;
    private bool _isDestory;
    private AudioSource _audioSource;
    private Coroutine _hideHPBarCoroutine;

    private void Awake()
    {
        _currentHP = maxHP;
        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
        }

        if (hpBarObject != null)
        {
            hpBarObject.SetActive(false);
        }
    }

    public void Interact(GameObject interactor)
    {
        _currentHP--;
        
        if(hitParticle != null) hitParticle.Play();
        if(hitSound != null) _audioSource.PlayOneShot(hitSound);

        UpdateHPBar();
        StartCoroutine(ShakeEffect());

        if (_currentHP <= 0)
            StartCoroutine(DestroyResource());
    }

    public void LockOn(bool isLockOn)
    {
        
    }

    private void UpdateHPBar()
    {
        if (hpSlider != null) hpSlider.value = _currentHP;
        if (hpBarObject != null) hpBarObject.SetActive(true);
        
        if(_hideHPBarCoroutine != null) StopCoroutine(_hideHPBarCoroutine);
        _hideHPBarCoroutine = StartCoroutine(HideHPBarAfterDelay(2f));
    }

    private IEnumerator HideHPBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hpBarObject != null) hpBarObject.SetActive(false);
    }

    private IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < 0.2f)
        {
            float x = Random.Range(-1f, 1f) * 0.05f;
            float z = Random.Range(-1f, 1f) * 0.05f;
            transform.position = originalPos + new Vector3(x, 0, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;
    }

    private IEnumerator DestroyResource()
    {
        _isDestory = true;
        if (hpBarObject != null) hpBarObject.SetActive(false);
        if (destroyParticle != null) destroyParticle.Play();
        if (destroySound != null) _audioSource.PlayOneShot(destroySound);

        DropLoot();

        yield return new WaitForSeconds(0.1f);
        
        if (visualObject != null)
        {
            var renderers = visualObject.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = false;
        
            var colliders = visualObject.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = false;
        }

        StartCoroutine(RespawnAfterDelay());
    }

    private void DropLoot()
    {
        if (lootTable == null || lootTable.entries == null) return;
        
        bool droppedAnything = false;

        foreach (LootTableDefinition.Entry entry in lootTable.entries)
        {
            if (Random.value <= entry.chance)
            {
                int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);

                for (int i = 0; i < amount; i++)
                {
                    Vector2 scatter = Random.insideUnitCircle * entry.scatterRadius;
                    Vector3 dropPos = transform.position + new Vector3(scatter.x, 0.5f, scatter.y);
                    Instantiate(entry.dropPrefab, dropPos, Quaternion.identity);
                    droppedAnything = true;
                }
            }
        }

        if (droppedAnything && lootTable.guaranteeAtLeastOne && lootTable.entries.Length > 0)
        {
            var entry = lootTable.entries[0];
            Instantiate(entry.dropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);

        _currentHP = maxHP;
        _isDestory = false;
        
        if (visualObject != null)
        {
            var renderers = visualObject.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = true;
        
            var colliders = visualObject.GetComponentsInChildren<Collider>();
            foreach (var c in colliders) c.enabled = true;
        }

        if (hpSlider != null) hpSlider.value = maxHP;
        Debug.Log($"[GatherableResource] {resourceName} 재생성!");
    }
    
}
