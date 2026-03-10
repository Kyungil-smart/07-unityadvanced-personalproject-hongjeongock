using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CharacterCustomizationUIController : MonoBehaviour
{
    [Header("UIDocument")]
    [SerializeField] private UIDocument uiDocument;

    [Header("미리보기 캐릭터")]
    [SerializeField] private PlayerCustomizer previewCustomizer;
    [SerializeField] private RenderTexture previewRenderTexture;

    [Header("완료 후 이동할 씬")]
    [SerializeField] private string nextSceneName = "MainScene";

    [Header("옵션 개수")]
    [SerializeField] private int bodyCount = 15;
    [SerializeField] private int hairCount = 12;
    [SerializeField] private int beardCount = 8;
    [SerializeField] private int hatCount = 40;
    [SerializeField] private int bagCount = 10;

    [Header("색상 개수")]
    [SerializeField] private int hairColorCount = 5;
    [SerializeField] private int hatColorCount = 5;
    
    [Header("미리보기 카메라")]
    [SerializeField] private Transform previewCamera;

    private VisualElement _root;

    private TextField _inputNickname;

    private Button _btnBodyPrev;
    private Button _btnBodyNext;
    private Label _valueBody;

    private Button _btnHairPrev;
    private Button _btnHairNext;
    private Label _valueHair;

    private Button _btnBeardPrev;
    private Button _btnBeardNext;
    private Label _valueBeard;

    private Button _btnHatPrev;
    private Button _btnHatNext;
    private Label _valueHat;

    private Button _btnBagPrev;
    private Button _btnBagNext;
    private Label _valueBag;

    private Button[] _hairColorButtons;
    private Button[] _hatColorButtons;

    private Button _btnCancel;
    private Button _btnRandom;
    private Button _btnComplete;

    private PlayerCustomizationData _workingData;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        InitData();
        CacheUI();
        BindEvents();
        RefreshAllUI();
        ApplyPreview();
    }

    private void OnDisable()
    {
        UnbindEvents();
    }

    private void InitData()
    {
        _workingData = new PlayerCustomizationData();

        if (PlayerProfileManager.Instance == null) return;

        PlayerCustomizationData saved = PlayerProfileManager.Instance.customizationData;

        _workingData.nickname = saved.nickname;
        _workingData.bodyIndex = saved.bodyIndex;
        _workingData.hairIndex = saved.hairIndex;
        _workingData.beardIndex = saved.beardIndex;
        _workingData.hatIndex = saved.hatIndex;
        _workingData.bagIndex = saved.bagIndex;
        _workingData.hairColorIndex = saved.hairColorIndex;
        _workingData.hatColorIndex = saved.hatColorIndex;
    }

    private void CacheUI()
    {
        _root = uiDocument.rootVisualElement;
        
        if (previewRenderTexture != null)
        {
            var previewPanel = _root.Q<VisualElement>("preview-box");
            if (previewPanel != null)
                previewPanel.style.backgroundImage = 
                    new StyleBackground(Background.FromRenderTexture(previewRenderTexture));
        }

        _inputNickname = _root.Q<TextField>("input-nickname");

        _btnBodyPrev = _root.Q<Button>("btn-body-prev");
        _btnBodyNext = _root.Q<Button>("btn-body-next");
        _valueBody = _root.Q<Label>("value-body");

        _btnHairPrev = _root.Q<Button>("btn-hair-prev");
        _btnHairNext = _root.Q<Button>("btn-hair-next");
        _valueHair = _root.Q<Label>("value-hair");

        _btnBeardPrev = _root.Q<Button>("btn-beard-prev");
        _btnBeardNext = _root.Q<Button>("btn-beard-next");
        _valueBeard = _root.Q<Label>("value-beard");

        _btnHatPrev = _root.Q<Button>("btn-hat-prev");
        _btnHatNext = _root.Q<Button>("btn-hat-next");
        _valueHat = _root.Q<Label>("value-hat");

        _btnBagPrev = _root.Q<Button>("btn-bag-prev");
        _btnBagNext = _root.Q<Button>("btn-bag-next");
        _valueBag = _root.Q<Label>("value-bag");

        _hairColorButtons = new Button[hairColorCount];
        for (int i = 0; i < hairColorCount; i++)
            _hairColorButtons[i] = _root.Q<Button>($"hair-color-{i}");

        _hatColorButtons = new Button[hatColorCount];
        for (int i = 0; i < hatColorCount; i++)
            _hatColorButtons[i] = _root.Q<Button>($"hat-color-{i}");

        _btnCancel = _root.Q<Button>("btn-cancel");
        _btnRandom = _root.Q<Button>("btn-random");
        _btnComplete = _root.Q<Button>("btn-complete");
    }

    private void BindEvents()
    {
        if (_inputNickname != null)
        {
            _inputNickname.SetValueWithoutNotify(_workingData.nickname);
            _inputNickname.RegisterValueChangedCallback(OnNicknameChanged);
        }

        if (_btnBodyPrev != null) _btnBodyPrev.clicked += OnClickBodyPrev;
        if (_btnBodyNext != null) _btnBodyNext.clicked += OnClickBodyNext;

        if (_btnHairPrev != null) _btnHairPrev.clicked += OnClickHairPrev;
        if (_btnHairNext != null) _btnHairNext.clicked += OnClickHairNext;

        if (_btnBeardPrev != null) _btnBeardPrev.clicked += OnClickBeardPrev;
        if (_btnBeardNext != null) _btnBeardNext.clicked += OnClickBeardNext;

        if (_btnHatPrev != null) _btnHatPrev.clicked += OnClickHatPrev;
        if (_btnHatNext != null) _btnHatNext.clicked += OnClickHatNext;

        if (_btnBagPrev != null) _btnBagPrev.clicked += OnClickBagPrev;
        if (_btnBagNext != null) _btnBagNext.clicked += OnClickBagNext;

        BindColorButtons(_hairColorButtons, OnClickHairColor);
        BindColorButtons(_hatColorButtons, OnClickHatColor);

        if (_btnCancel != null) _btnCancel.clicked += OnClickCancel;
        if (_btnRandom != null) _btnRandom.clicked += OnClickRandom;
        if (_btnComplete != null) _btnComplete.clicked += OnClickComplete;
    }

    private void UnbindEvents()
    {
        if (_inputNickname != null)
            _inputNickname.UnregisterValueChangedCallback(OnNicknameChanged);

        if (_btnBodyPrev != null) _btnBodyPrev.clicked -= OnClickBodyPrev;
        if (_btnBodyNext != null) _btnBodyNext.clicked -= OnClickBodyNext;

        if (_btnHairPrev != null) _btnHairPrev.clicked -= OnClickHairPrev;
        if (_btnHairNext != null) _btnHairNext.clicked -= OnClickHairNext;

        if (_btnBeardPrev != null) _btnBeardPrev.clicked -= OnClickBeardPrev;
        if (_btnBeardNext != null) _btnBeardNext.clicked -= OnClickBeardNext;

        if (_btnHatPrev != null) _btnHatPrev.clicked -= OnClickHatPrev;
        if (_btnHatNext != null) _btnHatNext.clicked -= OnClickHatNext;

        if (_btnBagPrev != null) _btnBagPrev.clicked -= OnClickBagPrev;
        if (_btnBagNext != null) _btnBagNext.clicked -= OnClickBagNext;

        if (_btnCancel != null) _btnCancel.clicked -= OnClickCancel;
        if (_btnRandom != null) _btnRandom.clicked -= OnClickRandom;
        if (_btnComplete != null) _btnComplete.clicked -= OnClickComplete;
    }

    private void BindColorButtons(Button[] buttons, System.Action<int> callback)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            if (buttons[i] == null) continue;
            buttons[i].clicked += () => callback(index);
        }
    }

    private void OnNicknameChanged(ChangeEvent<string> evt)
    {
        _workingData.nickname = evt.newValue;
    }

    private void OnClickBodyPrev()
    {
        _workingData.bodyIndex = LoopPrev(_workingData.bodyIndex, bodyCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickBodyNext()
    {
        _workingData.bodyIndex = LoopNext(_workingData.bodyIndex, bodyCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickHairPrev()
    {
        _workingData.hairIndex = LoopPrev(_workingData.hairIndex, hairCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickHairNext()
    {
        _workingData.hairIndex = LoopNext(_workingData.hairIndex, hairCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickBeardPrev()
    {
        _workingData.beardIndex = LoopPrev(_workingData.beardIndex, beardCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickBeardNext()
    {
        _workingData.beardIndex = LoopNext(_workingData.beardIndex, beardCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickHatPrev()
    {
        _workingData.hatIndex = LoopPrev(_workingData.hatIndex, hatCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickHatNext()
    {
        _workingData.hatIndex = LoopNext(_workingData.hatIndex, hatCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToFront();
    }

    private void OnClickBagPrev()
    {
        _workingData.bagIndex = LoopPrev(_workingData.bagIndex, bagCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToBack();
    }

    private void OnClickBagNext()
    {
        _workingData.bagIndex = LoopNext(_workingData.bagIndex, bagCount);
        RefreshAllUI();
        ApplyPreview();
        RotateCameraToBack();
    }
    private void RotateCameraToFront()
    {
        if (previewCamera == null) return;
        previewCamera.position = new Vector3(0, 1, 3);
        previewCamera.rotation = Quaternion.Euler(0, 180, 0); // 정면
    }

    private void RotateCameraToBack()
    {
        if (previewCamera == null) return;
        previewCamera.position = new Vector3(0, 1, -3);
        previewCamera.rotation = Quaternion.Euler(0, 360, 0); // 뒤
    }

    private void OnClickHairColor(int index)
    {
        _workingData.hairColorIndex = index;
        RefreshColorButtons();
        ApplyPreview();
    }

    private void OnClickHatColor(int index)
    {
        _workingData.hatColorIndex = index;
        RefreshColorButtons();
        ApplyPreview();
    }

    private void OnClickRandom()
    {
        _workingData.bodyIndex = Random.Range(0, Mathf.Max(1, bodyCount));
        _workingData.hairIndex = Random.Range(0, Mathf.Max(1, hairCount));
        _workingData.beardIndex = Random.Range(0, Mathf.Max(1, beardCount));
        _workingData.hatIndex = Random.Range(0, Mathf.Max(1, hatCount));
        _workingData.bagIndex = Random.Range(0, Mathf.Max(1, bagCount));

        _workingData.hairColorIndex = Random.Range(0, Mathf.Max(1, hairColorCount));
        _workingData.hatColorIndex = Random.Range(0, Mathf.Max(1, hatColorCount));

        RefreshAllUI();
        ApplyPreview();
    }

    private void OnClickCancel()
    {
        gameObject.SetActive(false);
    }

    private void OnClickComplete()
    {
        if (string.IsNullOrWhiteSpace(_workingData.nickname))
            _workingData.nickname = "Player";

        if (PlayerProfileManager.Instance != null)
        {
            PlayerProfileManager.Instance.SetCustomizationData(_workingData);
            PlayerProfileManager.Instance.hasFinishedCustomization = true;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void RefreshAllUI()
    {
        RefreshValueTexts();
        RefreshColorButtons();

        if (_inputNickname != null)
            _inputNickname.SetValueWithoutNotify(_workingData.nickname);
    }

    private void RefreshValueTexts()
    {
        if (_valueBody != null)
            _valueBody.text = GetIndexedText("바디", _workingData.bodyIndex);

        if (_valueHair != null)
            _valueHair.text = GetIndexedText("헤어", _workingData.hairIndex);

        if (_valueBeard != null)
            _valueBeard.text = GetIndexedText("수염", _workingData.beardIndex);

        if (_valueHat != null)
            _valueHat.text = GetIndexedText("모자", _workingData.hatIndex);

        if (_valueBag != null)
            _valueBag.text = GetIndexedText("가방", _workingData.bagIndex);
    }

    private void RefreshColorButtons()
    {
        RefreshSwatchGroup(_hairColorButtons, _workingData.hairColorIndex);
        RefreshSwatchGroup(_hatColorButtons, _workingData.hatColorIndex);
    }

    private void RefreshSwatchGroup(Button[] buttons, int selectedIndex)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            buttons[i].RemoveFromClassList("swatch-active");

            if (i == selectedIndex)
                buttons[i].AddToClassList("swatch-active");
        }
    }

    private void ApplyPreview()
    {
        if (previewCustomizer == null)
        {
            Debug.LogWarning("[CharacterCustomizationUIController] previewCustomizer is null");
            return;
        }

        previewCustomizer.ApplyCustomization(_workingData);
    }

    private int LoopPrev(int current, int max)
    {
        if (max <= 0) return 0;
        current--;
        if (current < 0) current = max - 1;
        return current;
    }

    private int LoopNext(int current, int max)
    {
        if (max <= 0) return 0;
        current++;
        if (current >= max) current = 0;
        return current;
    }

    private string GetIndexedText(string label, int index)
    {
        
        return $"{label} {index + 1}";
    }
}