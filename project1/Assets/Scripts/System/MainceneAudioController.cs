using UnityEngine;

public class MainSceneAudioController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private AudioManager audioManager;

    [Header("메인씬 BGM")]
    [SerializeField] private AudioClip mainSceneBgm;

    private void Start()
    {
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        if (audioManager != null)
            audioManager.PlayBgm(mainSceneBgm);
    }
    
}