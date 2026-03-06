using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("AudioSource 컴포넌트")]
    public AudioSource sfxSource;
    public AudioSource zombieSource; 

    [Header("플레이어 사운드")]
    public AudioClip playerAttackClip;
    public AudioClip playerHitClip;

    [Header("좀비 사운드")]
    public AudioClip zombieIdleClip;
    public AudioClip zombieAttackClip;

    [Header("아이템 사운드")]
    public AudioClip itemPickupClip;
    public AudioClip coinPickupClip;

    [Header("건물 사운드")]
    public AudioClip houseUpgradeClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayPlayerAttack()
    {
        sfxSource.PlayOneShot(playerAttackClip);
    }

    public void PlayPlayerHit()
    {
        sfxSource.PlayOneShot(playerHitClip);
    }
    
    public void PlayZombieIdle()
    {
        if (zombieSource.isPlaying) return;
        zombieSource.clip = zombieIdleClip;
        zombieSource.loop = true;
        zombieSource.Play();
    }

    public void StopZombieIdle()
    {
        zombieSource.Stop();
    }

    public void PlayZombieAttack()
    {
        sfxSource.PlayOneShot(zombieAttackClip);
    }
    
    public void PlayItemPickup()
    {
        sfxSource.PlayOneShot(itemPickupClip);
    }

    public void PlayCoinPickup()
    {
        sfxSource.PlayOneShot(coinPickupClip);
    }
    
    public void PlayHouseUpgrade()
    {
        sfxSource.PlayOneShot(houseUpgradeClip);
    }
}