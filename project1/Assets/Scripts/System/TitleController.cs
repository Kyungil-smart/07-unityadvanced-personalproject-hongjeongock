using UnityEngine;
using UnityEngine.UIElements;

public class TitleController : MonoBehaviour
{
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("[TitleController] UIDocument 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        var root = uiDocument.rootVisualElement;

        // 새 게임 버튼 연결
        var newGameBtn = root.Q<Button>("btn-newgame");
        if (newGameBtn != null)
            newGameBtn.clicked += OnNewGame;
        else
            Debug.LogError("[TitleController] 'btn-newgame' 버튼을 찾을 수 없습니다.");
    }

    private void OnDisable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;
        var newGameBtn = root.Q<Button>("btn-newgame");
        if (newGameBtn != null)
            newGameBtn.clicked -= OnNewGame;
    }

    private void OnNewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}