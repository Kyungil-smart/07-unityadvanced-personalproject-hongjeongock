using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsSoundUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UIDocument settingsDoc;
    [SerializeField] private AudioManager audioManager;

    [Header("UI 이름")]
    [SerializeField] private string bgmSliderName = "slider-bgm";
    [SerializeField] private string sfxSliderName = "slider-sfx";
    [SerializeField] private string applyButtonName = "btn-apply";

    private Slider _bgm;
    private Slider _sfx;
    private Button _apply;

    private void Awake()
    {
        if (settingsDoc == null)
            settingsDoc = GetComponent<UIDocument>();

        if (audioManager == null)
            audioManager = Object.FindFirstObjectByType<AudioManager>();

        if (settingsDoc == null || audioManager == null)
            return;

        var root = settingsDoc.rootVisualElement;

        _bgm = root.Q<Slider>(bgmSliderName);
        _sfx = root.Q<Slider>(sfxSliderName);
        _apply = root.Q<Button>(applyButtonName);

        if (_bgm != null)
            _bgm.RegisterValueChangedCallback(e => audioManager.ApplyBgmVolume(e.newValue));

        if (_sfx != null)
            _sfx.RegisterValueChangedCallback(e => audioManager.ApplySfxVolume(e.newValue));

        if (_apply != null)
            _apply.clicked += audioManager.SaveVolumes;
    }

    public void RefreshUI()
    {
        if (audioManager == null)
            audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager == null) return;

        if (_bgm != null)
        {
            _bgm.SetValueWithoutNotify(audioManager.BgmVolume);
            
            var root = settingsDoc.rootVisualElement;
            var labelBgm = root.Q<Label>("label-bgm");
            if (labelBgm != null)
                labelBgm.text = Mathf.RoundToInt(audioManager.BgmVolume * 100).ToString();
        }

        if (_sfx != null)
        {
            _sfx.SetValueWithoutNotify(audioManager.SfxVolume);
            var root = settingsDoc.rootVisualElement;
            var labelSfx = root.Q<Label>("label-sfx");
            if (labelSfx != null)
                labelSfx.text = Mathf.RoundToInt(audioManager.SfxVolume * 100).ToString();
        }
    }
    private void OnEnable()
    {
        if (settingsDoc == null)
            settingsDoc = GetComponent<UIDocument>();

        if (audioManager == null)
            audioManager = Object.FindFirstObjectByType<AudioManager>();

        if (settingsDoc == null || audioManager == null)
        {
            Debug.LogWarning("[SettingsSoundUI] UIDocument 또는 AudioManager를 찾지 못했습니다.");
            return;
        }

        var root = settingsDoc.rootVisualElement;

        _bgm = root.Q<Slider>(bgmSliderName);
        _sfx = root.Q<Slider>(sfxSliderName);
        _apply = root.Q<Button>(applyButtonName);

        Debug.Log($"[SettingsSoundUI] bgm:{_bgm} sfx:{_sfx} apply:{_apply}");

        if (_bgm != null)
        {
            _bgm.SetValueWithoutNotify(audioManager.BgmVolume);
            _bgm.RegisterValueChangedCallback(e => audioManager.ApplyBgmVolume(e.newValue));
        }

        if (_sfx != null)
        {
            _sfx.SetValueWithoutNotify(audioManager.SfxVolume);
            _sfx.RegisterValueChangedCallback(e => audioManager.ApplySfxVolume(e.newValue));
        }

        if (_apply != null)
            _apply.clicked += audioManager.SaveVolumes;
    }
}