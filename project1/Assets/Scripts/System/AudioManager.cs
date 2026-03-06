using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("초기 볼륨")]
    [Range(0f, 1f)] [SerializeField] private float defaultBgm = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float defaultSfx = 1f;

    [Header("UI 사운드")]
    [SerializeField] private AudioClip uiClickSound;

    private const string KEY_BGM = "VOL_BGM";
    private const string KEY_SFX = "VOL_SFX";

    public float BgmVolume => bgmSource != null ? bgmSource.volume : 0f;
    public float SfxVolume => sfxSource != null ? sfxSource.volume : 0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        float bgm = PlayerPrefs.GetFloat(KEY_BGM, defaultBgm);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, defaultSfx);
        ApplyBgmVolume(bgm);
        ApplySfxVolume(sfx);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(BindAllButtonsDelayed());
    }

    private IEnumerator BindAllButtonsDelayed()
    {
        yield return null;

        var allDocs = FindObjectsByType<UIDocument>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var doc in allDocs)
        {
            var root = doc.rootVisualElement;
            if (root == null) continue;

            foreach (var btn in root.Query<Button>().ToList())
            {
                btn.clicked -= PlayUIClick;
                btn.clicked += PlayUIClick;
            }
        }

        Debug.Log($"[AudioManager] 버튼 사운드 바인딩 완료");
    }
    
    public void ApplyBgmVolume(float value)
    {
        value = Mathf.Clamp01(value);
        if (bgmSource != null) bgmSource.volume = value;
        PlayerPrefs.SetFloat(KEY_BGM, value);
    }

    public void ApplySfxVolume(float value)
    {
        value = Mathf.Clamp01(value);
        if (sfxSource != null) sfxSource.volume = value;
        PlayerPrefs.SetFloat(KEY_SFX, value);
    }

    public void SaveVolumes()
    {
        PlayerPrefs.Save();
    }

    public void PlayUIClick()
    {
        if (this == null) return;
    
        if (sfxSource == null)
        {
            Debug.LogError("[AudioManager] sfxSource가 null! 재탐색 시도");
            sfxSource = GetComponentInChildren<AudioSource>();
            if (sfxSource == null) return;
        }
    
        if (uiClickSound == null) return;
        sfxSource.PlayOneShot(uiClickSound);
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}