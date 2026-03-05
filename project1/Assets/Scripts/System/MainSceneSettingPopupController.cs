using UnityEngine;
using UnityEngine.UIElements;

public class MainSceneSettingPopupController : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private UIDocument settingsDocument;

    private VisualElement _root;

    private Button _tabSound;
    private Button _tabKeys;
    private VisualElement _contentSound;
    private VisualElement _contentKeys;

    private Slider _sliderBGM;
    private Slider _sliderSFX;
    private Label _labelBGM;
    private Label _labelSFX;

    private Button _btnClose;
    private Button _btnApply;

    private bool _bound;

    private void OnEnable()
    {
        BindOnce();
        ShowSoundTabByDefault();
    }

    private void BindOnce()
    {
        if (_bound) return;

        if (settingsDocument == null)
            settingsDocument = GetComponent<UIDocument>();

        _root = settingsDocument.rootVisualElement;
        
        _tabSound     = _root.Q<Button>("tab-sound");
        _tabKeys      = _root.Q<Button>("tab-keys");
        _contentSound = _root.Q<VisualElement>("content-sound");
        _contentKeys  = _root.Q<VisualElement>("content-keys");

        _sliderBGM = _root.Q<Slider>("slider-bgm");
        _sliderSFX = _root.Q<Slider>("slider-sfx");
        _labelBGM  = _root.Q<Label>("label-bgm");
        _labelSFX  = _root.Q<Label>("label-sfx");

        _btnClose = _root.Q<Button>("btn-close");
        _btnApply = _root.Q<Button>("btn-apply");
        
        if (_tabSound == null) Debug.LogWarning("[SettingsPopup] tab-sound 못 찾음");
        if (_tabKeys  == null) Debug.LogWarning("[SettingsPopup] tab-keys 못 찾음");
        if (_contentSound == null) Debug.LogWarning("[SettingsPopup] content-sound 못 찾음");
        if (_contentKeys  == null) Debug.LogWarning("[SettingsPopup] content-keys 못 찾음");
        if (_btnClose == null) Debug.LogWarning("[SettingsPopup] btn-close 못 찾음");
        if (_btnApply == null) Debug.LogWarning("[SettingsPopup] btn-apply 못 찾음");
        
        if (_tabSound != null) _tabSound.clicked += () => SwitchTab(true);
        if (_tabKeys  != null) _tabKeys .clicked += () => SwitchTab(false);
        
        if (_btnClose != null) _btnClose.clicked += Close;
        if (_btnApply != null) _btnApply.clicked += Close;
        
        if (_sliderBGM != null)
            _sliderBGM.RegisterValueChangedCallback(e =>
            {
                if (_labelBGM != null) _labelBGM.text = Mathf.RoundToInt(e.newValue * 100).ToString();
            });

        if (_sliderSFX != null)
            _sliderSFX.RegisterValueChangedCallback(e =>
            {
                if (_labelSFX != null) _labelSFX.text = Mathf.RoundToInt(e.newValue * 100).ToString();
            });

        _bound = true;
    }

    private void ShowSoundTabByDefault()
    {
        SwitchTab(true);
    }

    private void SwitchTab(bool sound)
    {
        _tabSound?.RemoveFromClassList("tab-active");
        _tabKeys ?.RemoveFromClassList("tab-active");
        _contentSound?.AddToClassList("content-hidden");
        _contentKeys ?.AddToClassList("content-hidden");

        if (sound)
        {
            _tabSound?.AddToClassList("tab-active");
            _contentSound?.RemoveFromClassList("content-hidden");
        }
        else
        {
            _tabKeys?.AddToClassList("tab-active");
            _contentKeys?.RemoveFromClassList("content-hidden");
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}