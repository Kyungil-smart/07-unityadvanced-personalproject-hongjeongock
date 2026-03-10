using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 레벨업 시 강화 선택 UI를 열고,
/// 현재 장착 무기의 업그레이드 옵션 3개를 표시하는 스크립트
/// </summary>
public class LevelUpSelectionUI : MonoBehaviour
{
    [Header("UI 문서")]
    [SerializeField] private UIDocument uiDocument;

    [Header("일시정지 여부")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private VisualElement _root;
    private VisualElement _panelRoot;

    // 카드 3개
    private Button _optionButton1;
    private Button _optionButton2;
    private Button _optionButton3;

    // 카드 내부 텍스트
    private Label _nameLabel1;
    private Label _descLabel1;

    private Label _nameLabel2;
    private Label _descLabel2;

    private Label _nameLabel3;
    private Label _descLabel3;

    // 현재 표시 중인 옵션 목록
    private List<UpgradeOptionData> _currentOptions = new List<UpgradeOptionData>();

    // 중복 바인딩 방지
    private bool _isBound;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        InitUI();
        Hide();
    }

    /// <summary>
    /// UI 요소를 한 번만 찾아서 연결
    /// </summary>
    private void InitUI()
    {
        if (uiDocument == null)
        {
            Debug.LogError("[LevelUpSelectionUI] UIDocument가 연결되지 않았습니다.");
            return;
        }

        _root = uiDocument.rootVisualElement;

        // 전체 패널
        _panelRoot = _root.Q<VisualElement>("LevelUpPanel");

        // 카드 버튼 3개
        _optionButton1 = _root.Q<Button>("OptionButton1");
        _optionButton2 = _root.Q<Button>("OptionButton2");
        _optionButton3 = _root.Q<Button>("OptionButton3");

        // 카드 1
        _nameLabel1 = _root.Q<Label>("OptionName1");
        _descLabel1 = _root.Q<Label>("OptionDesc1");

        // 카드 2
        _nameLabel2 = _root.Q<Label>("OptionName2");
        _descLabel2 = _root.Q<Label>("OptionDesc2");

        // 카드 3
        _nameLabel3 = _root.Q<Label>("OptionName3");
        _descLabel3 = _root.Q<Label>("OptionDesc3");

        // 필수 요소 체크
        if (_panelRoot == null ||
            _optionButton1 == null || _optionButton2 == null || _optionButton3 == null ||
            _nameLabel1 == null || _descLabel1 == null ||
            _nameLabel2 == null || _descLabel2 == null ||
            _nameLabel3 == null || _descLabel3 == null)
        {
            Debug.LogError("[LevelUpSelectionUI] UXML 이름이 스크립트와 맞지 않습니다.");
            return;
        }

        BindButtons();
    }

    /// <summary>
    /// 버튼 클릭 이벤트 연결
    /// </summary>
    private void BindButtons()
    {
        if (_isBound) return;

        _optionButton1.clicked += () => OnOptionSelected(0);
        _optionButton2.clicked += () => OnOptionSelected(1);
        _optionButton3.clicked += () => OnOptionSelected(2);

        _isBound = true;
    }

    /// <summary>
    /// 레벨업 UI 열기
    /// </summary>
    public void Open()
    {
        if (WeaponManager.Instance == null)
        {
            Debug.LogError("[LevelUpSelectionUI] WeaponManager 인스턴스가 없습니다.");
            return;
        }

        // 현재 장착 무기 기준으로 랜덤 3개 뽑기
        _currentOptions = WeaponManager.Instance.GetRandomUpgradeOptions(3);

        if (_currentOptions == null || _currentOptions.Count == 0)
        {
            Debug.LogWarning("[LevelUpSelectionUI] 표시할 업그레이드 옵션이 없습니다.");
            return;
        }

        RefreshUI();

        if (_panelRoot != null)
            _panelRoot.style.display = DisplayStyle.Flex;

        if (pauseGameWhenOpen)
            Time.timeScale = 0f;
    }

    /// <summary>
    /// 레벨업 UI 닫기
    /// </summary>
    public void Close()
    {
        Hide();

        if (pauseGameWhenOpen)
            Time.timeScale = 1f;
    }

    /// <summary>
    /// 처음 시작 시 숨김 처리
    /// </summary>
    private void Hide()
    {
        if (_panelRoot != null)
            _panelRoot.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// 현재 뽑힌 업그레이드 3개를 UI에 반영
    /// </summary>
    private void RefreshUI()
    {
        // 옵션이 3개보다 적게 뽑히는 경우도 대비
        SetOptionUI(0, _optionButton1, _nameLabel1, _descLabel1);
        SetOptionUI(1, _optionButton2, _nameLabel2, _descLabel2);
        SetOptionUI(2, _optionButton3, _nameLabel3, _descLabel3);
    }

    /// <summary>
    /// 각 카드 UI 갱신
    /// </summary>
    private void SetOptionUI(int index, Button button, Label nameLabel, Label descLabel)
    {
        if (button == null || nameLabel == null || descLabel == null)
            return;

        if (index >= _currentOptions.Count || _currentOptions[index] == null)
        {
            button.style.display = DisplayStyle.None;
            return;
        }

        UpgradeOptionData option = _currentOptions[index];

        button.style.display = DisplayStyle.Flex;
        nameLabel.text = option.optionName;
        descLabel.text = option.description;

        // 아이콘까지 쓸 거면 UXML에 VisualElement를 따로 두고
        // backgroundImage로 추가 연결하면 된다.
    }

    /// <summary>
    /// 옵션 선택 처리
    /// </summary>
    private void OnOptionSelected(int index)
    {
        if (WeaponManager.Instance == null) return;
        if (_currentOptions == null) return;
        if (index < 0 || index >= _currentOptions.Count) return;
        if (_currentOptions[index] == null) return;

        UpgradeOptionData selectedOption = _currentOptions[index];

        // 실제 업그레이드 적용
        WeaponManager.Instance.ApplyUpgrade(selectedOption);

        Debug.Log($"[LevelUpSelectionUI] 선택한 강화: {selectedOption.optionName}");

        // UI 닫기
        Close();
    }
}