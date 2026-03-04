using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PauseController : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private UIDocument uiDocument;

    private VisualElement root;

    private void Start()
    {
        root = uiDocument.rootVisualElement;

        root.Q<Button>("btn-resume").clicked   += Resume;
        root.Q<Button>("btn-settings").clicked += OnSettings;
        root.Q<Button>("btn-mainmenu").clicked += GoMainMenu;
        root.Q<Button>("btn-quit").clicked     += QuitGame;

        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("게임 재개");
    }

    public void Pause()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("게임 일시정지");
    }

    private void OnSettings()
    {
        Debug.Log("설정 (미구현)");
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}