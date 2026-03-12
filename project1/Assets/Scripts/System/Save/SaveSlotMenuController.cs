using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveSlotMenuController : MonoBehaviour
{
    [Header("씬 이름")]
    [SerializeField] private string mainSceneName = "MainScene";
    [SerializeField] private string customizeSceneName = "CharacterCustomize"; // ← 추가

    [Header("기본 새 게임 값")]
    [SerializeField] private float defaultPlayerHp = 100f;
    [SerializeField] private int defaultGold = 0;
    [SerializeField] private int defaultHouseLevel = 1;
    
    [Header("새 게임 시작 위치")]
    [SerializeField] private Vector3 defaultStartPosition = new Vector3(-27.9552841f,-1.53746998f,-0.0435905457f);

    private UIDocument _uiDocument;
    private VisualElement _root;
    private bool _isNewGameMode = false;

    // 닫기 버튼
    private Button _btnClose;

    // 슬롯 1
    private Label _slot1State;
    private Label _slot1Nickname;
    private Label _slot1Progress;
    private Label _slot1Time;
    private Button _slot1LoadButton;
    private Button _slot1NewButton;

    // 슬롯 2
    private Label _slot2State;
    private Label _slot2Nickname;
    private Label _slot2Progress;
    private Label _slot2Time;
    private Button _slot2LoadButton;
    private Button _slot2NewButton;

    // 슬롯 3
    private Label _slot3State;
    private Label _slot3Nickname;
    private Label _slot3Progress;
    private Label _slot3Time;
    private Button _slot3LoadButton;
    private Button _slot3NewButton;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        if (_uiDocument == null)
        {
            Debug.LogError("SaveSlotMenuController: UIDocument가 없습니다.");
            return;
        }

        _root = _uiDocument.rootVisualElement;

        CacheUI();
        BindButtons();
        RefreshAllSlots();
    }

    private void OnDisable()
    {
        UnbindButtons();
    }

    private void CacheUI()
    {
        _btnClose = _root.Q<Button>("btn-save-close");

        _slot1State = _root.Q<Label>("txt-slot-1-state");
        _slot1Nickname = _root.Q<Label>("txt-slot-1-nickname");
        _slot1Progress = _root.Q<Label>("txt-slot-1-day");
        _slot1Time = _root.Q<Label>("txt-slot-1-time");
        _slot1LoadButton = _root.Q<Button>("btn-slot-1-load");
        _slot1NewButton = _root.Q<Button>("btn-slot-1-new");

        _slot2State = _root.Q<Label>("txt-slot-2-state");
        _slot2Nickname = _root.Q<Label>("txt-slot-2-nickname");
        _slot2Progress = _root.Q<Label>("txt-slot-2-day");
        _slot2Time = _root.Q<Label>("txt-slot-2-time");
        _slot2LoadButton = _root.Q<Button>("btn-slot-2-load");
        _slot2NewButton = _root.Q<Button>("btn-slot-2-new");

        _slot3State = _root.Q<Label>("txt-slot-3-state");
        _slot3Nickname = _root.Q<Label>("txt-slot-3-nickname");
        _slot3Progress = _root.Q<Label>("txt-slot-3-day");
        _slot3Time = _root.Q<Label>("txt-slot-3-time");
        _slot3LoadButton = _root.Q<Button>("btn-slot-3-load");
        _slot3NewButton = _root.Q<Button>("btn-slot-3-new");
    }

    private void BindButtons()
    {
        if (_btnClose != null)
        {
            _btnClose.clicked -= CloseMenu;
            _btnClose.clicked += CloseMenu;
        }

        if (_slot1LoadButton != null) { _slot1LoadButton.clicked -= OnClickLoadSlot1; _slot1LoadButton.clicked += OnClickLoadSlot1; }
        if (_slot1NewButton != null)  { _slot1NewButton.clicked  -= OnClickNewSlot1;  _slot1NewButton.clicked  += OnClickNewSlot1; }
        if (_slot2LoadButton != null) { _slot2LoadButton.clicked -= OnClickLoadSlot2; _slot2LoadButton.clicked += OnClickLoadSlot2; }
        if (_slot2NewButton != null)  { _slot2NewButton.clicked  -= OnClickNewSlot2;  _slot2NewButton.clicked  += OnClickNewSlot2; }
        if (_slot3LoadButton != null) { _slot3LoadButton.clicked -= OnClickLoadSlot3; _slot3LoadButton.clicked += OnClickLoadSlot3; }
        if (_slot3NewButton != null)  { _slot3NewButton.clicked  -= OnClickNewSlot3;  _slot3NewButton.clicked  += OnClickNewSlot3; }
    }

    private void UnbindButtons()
    {
        if (_btnClose != null)        _btnClose.clicked        -= CloseMenu;
        if (_slot1LoadButton != null) _slot1LoadButton.clicked -= OnClickLoadSlot1;
        if (_slot1NewButton != null)  _slot1NewButton.clicked  -= OnClickNewSlot1;
        if (_slot2LoadButton != null) _slot2LoadButton.clicked -= OnClickLoadSlot2;
        if (_slot2NewButton != null)  _slot2NewButton.clicked  -= OnClickNewSlot2;
        if (_slot3LoadButton != null) _slot3LoadButton.clicked -= OnClickLoadSlot3;
        if (_slot3NewButton != null)  _slot3NewButton.clicked  -= OnClickNewSlot3;
    }

    private void RefreshAllSlots()
    {
        RefreshSlotUI(1, _slot1State, _slot1Nickname, _slot1Progress, _slot1Time, _slot1LoadButton);
        RefreshSlotUI(2, _slot2State, _slot2Nickname, _slot2Progress, _slot2Time, _slot2LoadButton);
        RefreshSlotUI(3, _slot3State, _slot3Nickname, _slot3Progress, _slot3Time, _slot3LoadButton);
    }

    private void RefreshSlotUI(int slot, Label stateLabel, Label nicknameLabel,
        Label progressLabel, Label timeLabel, Button loadButton)
    {
        bool hasSave = SaveManager.HasSave(slot);

        if (!hasSave)
        {
            if (stateLabel != null)   stateLabel.text   = "빈 슬롯";
            if (nicknameLabel != null) nicknameLabel.text = "이름: -";
            if (progressLabel != null) progressLabel.text = "진행도: -";
            if (timeLabel != null)    timeLabel.text    = "저장 시간: -";
            if (loadButton != null)   loadButton.SetEnabled(false);
            return;
        }

        SaveData data = SaveManager.Load(slot);

        if (data == null)
        {
            if (stateLabel != null)   stateLabel.text   = "손상됨";
            if (nicknameLabel != null) nicknameLabel.text = "이름: -";
            if (progressLabel != null) progressLabel.text = "진행도: -";
            if (timeLabel != null)    timeLabel.text    = "저장 시간: -";
            if (loadButton != null)   loadButton.SetEnabled(false);
            return;
        }

        if (stateLabel != null)    stateLabel.text    = "사용 중";
        if (nicknameLabel != null) nicknameLabel.text = $"이름: {data.nickname}";
        if (progressLabel != null) progressLabel.text = $"진행도: 집 {data.houseLevel}레벨 / 골드 {data.gold}";
        if (timeLabel != null)     timeLabel.text     = $"저장 시간: {data.saveTime}";

        if (loadButton != null)
            loadButton.SetEnabled(!_isNewGameMode);
    }
    
    public void SetNewGameMode(bool isNewGame)
    {
        _isNewGameMode = isNewGame;
        RefreshAllSlots();
    }

    private SaveData CreateNewSaveData()
    {
        SaveData data = new SaveData();
        data.nickname    = PlayerPrefs.GetString("PlayerNickname", "플레이어");
        data.playerHp    = defaultPlayerHp;
        data.posX        = defaultStartPosition.x;
        data.posY        = defaultStartPosition.y;
        data.posZ        = defaultStartPosition.z;
        data.gold        = defaultGold;
        data.houseLevel  = defaultHouseLevel;
        return data;
    }

    /// <summary>
    /// 슬롯에 새 게임 데이터를 저장하고 커스터마이징 씬으로 이동
    /// </summary>
    private void StartNewGameInSlot(int slot)
    {
        SaveData newData = CreateNewSaveData();
        SaveManager.Save(slot, newData);

        PlayerPrefs.SetInt("LastSelectedSlot", slot);
        PlayerPrefs.SetInt("IsNewGame", 1);
        PlayerPrefs.Save();

        Debug.Log($"슬롯 {slot} 새 게임 → 커스터마이징 씬으로 이동");

        // ↓ 핵심 변경: MainScene 대신 CharacterCustomize로 이동
        SceneManager.LoadScene(customizeSceneName);
    }

    private void LoadGameFromSlot(int slot)
    {
        if (!SaveManager.HasSave(slot))
        {
            Debug.Log($"슬롯 {slot}에 저장 데이터가 없습니다.");
            RefreshAllSlots();
            return;
        }

        PlayerPrefs.SetInt("LastSelectedSlot", slot);
        PlayerPrefs.Save();

        Debug.Log($"슬롯 {slot} 불러오기 → MainScene으로 이동");
        SceneManager.LoadScene(mainSceneName);
    }

    private void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    #region 슬롯 버튼
    private void OnClickLoadSlot1() => LoadGameFromSlot(1);
    private void OnClickNewSlot1()  => StartNewGameInSlot(1);
    private void OnClickLoadSlot2() => LoadGameFromSlot(2);
    private void OnClickNewSlot2()  => StartNewGameInSlot(2);
    private void OnClickLoadSlot3() => LoadGameFromSlot(3);
    private void OnClickNewSlot3()  => StartNewGameInSlot(3);
    #endregion
}