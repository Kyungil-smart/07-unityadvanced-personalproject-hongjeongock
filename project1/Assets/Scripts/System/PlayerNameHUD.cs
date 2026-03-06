using UnityEngine;
using UnityEngine.UIElements;

public class PlayerNameHUD : MonoBehaviour
{
    [Header("HUD UI")]
    [SerializeField] private UIDocument uiDocument;

    private Label _playerNameLabel;

    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;
        
        _playerNameLabel = root.Q<Label>("player-name");

        if (_playerNameLabel == null)
        {
            Debug.LogWarning("player-name Label을 찾지 못했습니다.");
            return;
        }

        ApplyNickname();
    }
    
    private void ApplyNickname()
    {
        string nickname = PlayerPrefs.GetString("PlayerNickname", "플레이어");
        _playerNameLabel.text = nickname;
    }
}