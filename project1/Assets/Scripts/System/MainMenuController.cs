using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [Header("UIDocument 연결")]
    [SerializeField] private UIDocument mainMenuDocument;
    [SerializeField] private UIDocument settingsDocument;
    [SerializeField] private UIDocument creditsDocument;

    [Header("GameObject 연결")]
    [SerializeField] private GameObject creditObject;

    private VisualElement _mainRoot;
    private TextField _nicknameInput;

    private VisualElement _settingsRoot;
    private Button _tabSound;
    private Button _tabKeys;
    private VisualElement _contentSound;
    private VisualElement _contentKeys;
    private Slider _sliderBGM;
    private Slider _sliderSFX;
    private Label _labelBGM;
    private Label _labelSFX;

    private void Start()
    {
        _mainRoot = mainMenuDocument.rootVisualElement;
        _nicknameInput = _mainRoot.Q<TextField>("nickname-input");

        _mainRoot.Q<Button>("btn-start").clicked += OnStartClicked;
        _mainRoot.Q<Button>("btn-settings").clicked += OnSettingsClicked;
        _mainRoot.Q<Button>("btn-credits").clicked += OnCreditsClicked;
        _mainRoot.Q<Button>("btn-quit").clicked += OnQuitClicked;
        
        _settingsRoot = settingsDocument.rootVisualElement;

        _tabSound     = _settingsRoot.Q<Button>("tab-sound");
        _tabKeys      = _settingsRoot.Q<Button>("tab-keys");
        _contentSound = _settingsRoot.Q<VisualElement>("content-sound");
        _contentKeys  = _settingsRoot.Q<VisualElement>("content-keys");
        _sliderBGM    = _settingsRoot.Q<Slider>("slider-bgm");
        _sliderSFX    = _settingsRoot.Q<Slider>("slider-sfx");
        _labelBGM     = _settingsRoot.Q<Label>("label-bgm");
        _labelSFX     = _settingsRoot.Q<Label>("label-sfx");

        var btnClose = _settingsRoot.Q<Button>("btn-close");
        var btnApply = _settingsRoot.Q<Button>("btn-apply");

        if (btnClose != null) btnClose.clicked += OnCloseSettingsClicked;
        if (btnApply != null) btnApply.clicked += OnCloseSettingsClicked;

        if (_tabSound != null) _tabSound.clicked += () => SwitchTab("sound");
        if (_tabKeys != null) _tabKeys.clicked += () => SwitchTab("keys");

        if (_sliderBGM != null)
            _sliderBGM.RegisterValueChangedCallback(e =>
            {
                if (_labelBGM != null)
                    _labelBGM.text = Mathf.RoundToInt(e.newValue * 100).ToString();
            });

        if (_sliderSFX != null)
            _sliderSFX.RegisterValueChangedCallback(e =>
            {
                if (_labelSFX != null)
                    _labelSFX.text = Mathf.RoundToInt(e.newValue * 100).ToString();
            });
        
        _settingsRoot.style.display = DisplayStyle.None;

        if (creditObject != null)
            creditObject.SetActive(false);

        if (PlayerPrefs.HasKey("PlayerNickname") && _nicknameInput != null)
            _nicknameInput.value = PlayerPrefs.GetString("PlayerNickname");
    }

    private void OnStartClicked()
    {
        string nick = _nicknameInput?.value.Trim();
        if (string.IsNullOrWhiteSpace(nick))
        {
            _nicknameInput?.AddToClassList("input-error");
            _nicknameInput?.schedule.Execute(() =>
                _nicknameInput.RemoveFromClassList("input-error")).StartingIn(1800);
            return;
        }

        PlayerPrefs.SetString("PlayerNickname", nick);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }

    private void OnSettingsClicked()
    {
        _settingsRoot.style.display = DisplayStyle.Flex;
        
        SwitchTab("sound");
        RefreshSoundLabels();
    }

    private void OnCloseSettingsClicked()
    {
        _settingsRoot.style.display = DisplayStyle.None;
    }

    private void RefreshSoundLabels()
    {
        if (_sliderBGM != null && _labelBGM != null)
            _labelBGM.text = Mathf.RoundToInt(_sliderBGM.value * 100).ToString();

        if (_sliderSFX != null && _labelSFX != null)
            _labelSFX.text = Mathf.RoundToInt(_sliderSFX.value * 100).ToString();
    }

    private void OnCreditsClicked()
    {
        creditObject.SetActive(true);
        StartCoroutine(InitCredits());
    }

    private System.Collections.IEnumerator InitCredits()
    {
        yield return null;
        var creditsRoot = creditsDocument.rootVisualElement;

        var closeTop = creditsRoot.Q<Button>("btn-close-top");
        var closeBottom = creditsRoot.Q<Button>("btn-close-bottom");
        if (closeTop != null) closeTop.clicked += OnCloseCreditsClicked;
        if (closeBottom != null) closeBottom.clicked += OnCloseCreditsClicked;
    }

    private void OnCloseCreditsClicked()
    {
        creditObject.SetActive(false);
    }

    private void SwitchTab(string tabName)
    {
        _tabSound?.RemoveFromClassList("tab-active");
        _tabKeys?.RemoveFromClassList("tab-active");
        _contentSound?.AddToClassList("content-hidden");
        _contentKeys?.AddToClassList("content-hidden");

        if (tabName == "sound")
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

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}