using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 및 게임 흐름을 관리하는 싱글톤 매니저
/// 씬 이름은 Build Settings에 등록된 이름과 일치해야 합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("씬 이름 설정")]
    [SerializeField] private string titleSceneName = "MainMenu";
    [SerializeField] private string customizeSceneName = "CharacterCustomize";
    [SerializeField] private string gameSceneName = "MainScene";

    [Header("캐릭터 데이터 (ScriptableObject 연결)")]
    [SerializeField] public CharacterData characterData;

    private void Awake()
    {
        // 싱글톤: 씬이 바뀌어도 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ──────────────────────────────────────────
    //  씬 전환
    // ──────────────────────────────────────────

    /// <summary>새 게임 시작 → 커스터마이징 씬으로</summary>
    public void StartNewGame()
    {
        characterData.Reset();
        SceneManager.LoadScene(customizeSceneName);
    }

    /// <summary>커스터마이징 완료 → 인게임으로</summary>
    public void OnCustomizationComplete()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>타이틀로 복귀</summary>
    public void GoToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
