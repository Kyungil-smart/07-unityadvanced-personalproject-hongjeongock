using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverUI : MonoBehaviour
{
    [Header("씬 이름")]
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UXML name 값")]
    [SerializeField] private string overlayName = "overlay";
    [SerializeField] private string retryButtonName = "btn-retry"; 
    [SerializeField] private string mainButtonName  = "btn-main";

    private UIDocument _doc;
    private VisualElement _overlay;
    private Button _btnRetry;
    private Button _btnMain;

    private Coroutine _initRoutine;

    private void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null)
            Debug.LogError("[GameOverUI] UIDocument가 같은 오브젝트에 없습니다.");
    }

    private void OnEnable()
    {
        if (_doc == null) return;
        
        if (_initRoutine != null) StopCoroutine(_initRoutine);
        _initRoutine = StartCoroutine(InitAfterLayout());
    }

    private IEnumerator InitAfterLayout()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        CacheUI();
        Hide();
    }

    private void CacheUI()
    {
        var root = _doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[GameOverUI] rootVisualElement가 null 입니다. PanelSettings/UIDocument 상태 확인.");
            return;
        }
        
        Debug.Log($"[GameOverUI] root.childCount = {root.childCount}");

        _overlay = root.Q<VisualElement>(overlayName);

        if (_overlay == null)
        {
            Debug.LogError($"[GameOverUI] Show() 실패: '{overlayName}'를 못 찾았습니다. (UXML name 값/SourceAsset/복제본 VTA 확인)");
            return;
        }
        
        _btnRetry = root.Q<Button>(retryButtonName);
        _btnMain  = root.Q<Button>(mainButtonName);

        if (_btnRetry != null)
        {
            _btnRetry.clicked -= OnClickRetry;
            _btnRetry.clicked += OnClickRetry;
        }
        else Debug.LogWarning($"[GameOverUI] '{retryButtonName}' 버튼을 못 찾음 (UXML name 확인).");

        if (_btnMain != null)
        {
            _btnMain.clicked -= OnClickMain;
            _btnMain.clicked += OnClickMain;
        }
        else Debug.LogWarning($"[GameOverUI] '{mainButtonName}' 버튼을 못 찾음 (UXML name 확인).");
    }
    
    public void Show()
    {
        if (_overlay == null) CacheUI();
        if (_overlay == null) return;

        _overlay.style.display = DisplayStyle.Flex;

        Time.timeScale = 0f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
    }

    public void Hide()
    {
        if (_overlay != null)
            _overlay.style.display = DisplayStyle.None;

        Time.timeScale = 1f;
    }

    private void OnClickRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnClickMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}