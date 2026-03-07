using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("AudioSource 컴포넌트")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource zombieSource;

    [Header("플레이어 사운드")]
    [SerializeField] private AudioClip playerAttackClip;
    [SerializeField] private AudioClip playerHitClip;

    [Header("좀비 사운드")]
    [SerializeField] private AudioClip zombieIdleClip;
    [SerializeField] private AudioClip zombieAttackClip;

    [Header("아이템 사운드")]
    [SerializeField] private AudioClip itemPickupClip;
    [SerializeField] private AudioClip coinPickupClip;

    [Header("건물 사운드")]
    [SerializeField] private AudioClip houseUpgradeClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null || zombieSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length > 0 && sfxSource == null)
                sfxSource = sources[0];

            if (sources.Length > 1 && zombieSource == null)
                zombieSource = sources[1];
        }
    }

    public void PlayPlayerAttack()
    {
        if (sfxSource == null)
        {
            Debug.LogError("[SoundManager] sfxSource가 비어있음");
            return;
        }

        if (playerAttackClip == null)
        {
            Debug.LogError("[SoundManager] playerAttackClip이 비어있음");
            return;
        }

        sfxSource.PlayOneShot(playerAttackClip);
    }

    public void PlayPlayerHit()
    {
        if (sfxSource == null || playerHitClip == null) return;
        sfxSource.PlayOneShot(playerHitClip);
    }

    public void PlayZombieIdle()
    {
        if (zombieSource == null || zombieIdleClip == null) return;
        if (zombieSource.isPlaying) return;

        zombieSource.clip = zombieIdleClip;
        zombieSource.loop = true;
        zombieSource.Play();
    }

    public void StopZombieIdle()
    {
        if (zombieSource == null) return;
        zombieSource.Stop();
    }

    public void PlayZombieAttack()
    {
        if (sfxSource == null || zombieAttackClip == null) return;
        sfxSource.PlayOneShot(zombieAttackClip);
    }

    public void PlayItemPickup()
    {
        if (sfxSource == null || itemPickupClip == null) return;
        sfxSource.PlayOneShot(itemPickupClip);
    }

    public void PlayCoinPickup()
    {
        if (sfxSource == null || coinPickupClip == null) return;
        sfxSource.PlayOneShot(coinPickupClip);
    }

    public void PlayHouseUpgrade()
    {
        if (sfxSource == null || houseUpgradeClip == null) return;
        sfxSource.PlayOneShot(houseUpgradeClip);
    }
}