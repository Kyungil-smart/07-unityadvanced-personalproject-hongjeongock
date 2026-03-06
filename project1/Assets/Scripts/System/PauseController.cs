using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PauseController : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("퍼즈")]
    [SerializeField] private UIDocument uiDocument;

    [Header("설정")]
    [SerializeField] private UIDocument settingsDocument;
    
    [Header("입력 게이트")]
    [SerializeField] private PlayerInputGate inputGate;
    
    [Header("리스폰")]
    [SerializeField] private PlayerRespawn playerRespawn;

    private Button _btnRespawn;

    private VisualElement _pauseRoot;
    private VisualElement _settingsRoot;

    private void Start()
    {
        _pauseRoot = uiDocument.rootVisualElement;
        _settingsRoot = settingsDocument.rootVisualElement;

        _pauseRoot.style.display = DisplayStyle.None;
        _settingsRoot.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        StartCoroutine(InitNextFrame());
    }

    private IEnumerator InitNextFrame()
    {
        yield return null;

        if (uiDocument == null)
        {
            Debug.LogError("PauseController: uiDocument가 연결되지 않았습니다.");
            yield break;
        }

        _pauseRoot = uiDocument.rootVisualElement;
        if (_pauseRoot == null)
        {
            Debug.LogError("PauseController: uiDocument.rootVisualElement가 null입니다. UIDocument/PanelSettings 확인.");
            yield break;
        }

        if (settingsDocument != null)
            _settingsRoot = settingsDocument.rootVisualElement;
        
        var btnResume   = _pauseRoot.Q<Button>("btn-resume");
        var btnSettings = _pauseRoot.Q<Button>("btn-settings");
        var btnMainMenu = _pauseRoot.Q<Button>("btn-mainmenu");
        var btnQuit     = _pauseRoot.Q<Button>("btn-quit");
        _btnRespawn     = _pauseRoot.Q<Button>("btn-respawn");

        if (btnResume == null)   Debug.LogError("btn-resume 못 찾음");
        if (btnSettings == null) Debug.LogError("btn-settings 못 찾음");
        if (btnMainMenu == null) Debug.LogError("btn-mainmenu 못 찾음");
        if (btnQuit == null)     Debug.LogError("btn-quit 못 찾음");
        if (_btnRespawn == null) Debug.LogError("btn-respawn 못 찾음");

        if (btnResume != null)   btnResume.clicked += Resume;
        if (btnSettings != null) btnSettings.clicked += ToggleSettings;
        if (btnMainMenu != null) btnMainMenu.clicked += GoMainMenu;
        if (btnQuit != null)     btnQuit.clicked += QuitGame;
        if (_btnRespawn != null) _btnRespawn.clicked += OnClickRespawn;
        
        _pauseRoot.style.display = DisplayStyle.None;
        if (_settingsRoot != null)
            _settingsRoot.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC detected in PauseController");

            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        _pauseRoot.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        GameIsPaused = true;

        inputGate?.Lock();
    }

    public void Resume()
    {
        _pauseRoot.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (_settingsRoot != null)
            _settingsRoot.style.display = DisplayStyle.None;

        inputGate?.Unlock();
    }

    private void ToggleSettings()
    {
        if (_settingsRoot == null) return;

        bool isOpen = _settingsRoot.style.display == DisplayStyle.Flex;

        _settingsRoot.style.display =
            isOpen ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void OnClickRespawn()
    {
        Debug.Log("리스폰 버튼 클릭됨");

        if (playerRespawn == null)
        {
            Debug.LogWarning("PauseController: playerRespawn이 연결되지 않았습니다.");
            return;
        }

        playerRespawn.RespawnToSafePoint();
        Resume();
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}