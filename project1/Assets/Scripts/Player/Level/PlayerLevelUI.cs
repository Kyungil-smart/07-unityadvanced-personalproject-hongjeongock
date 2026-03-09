using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLevelUI : MonoBehaviour
{
    [Header("UIDocument")]
    [SerializeField] private UIDocument uiDocument;

    [Header("플레이어 레벨 시스템")]
    [SerializeField] private PlayerLevelSystem playerLevelSystem;

    private VisualElement _root;
    private VisualElement _xpFill;
    private Label _xpValueLabel;

    private void OnEnable()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        _root = uiDocument.rootVisualElement;

        _xpFill = _root.Q<VisualElement>("xp-fill");
        _xpValueLabel = _root.Q<Label>("xp-value");

        if (playerLevelSystem != null)
            playerLevelSystem.OnXPChanged += RefreshUI;
    }

    private void Start()
    {
        if (playerLevelSystem != null)
            RefreshUI(playerLevelSystem.currentLevel, playerLevelSystem.currentXP, playerLevelSystem.maxXP);
    }

    private void OnDisable()
    {
        if (playerLevelSystem != null)
            playerLevelSystem.OnXPChanged -= RefreshUI;
    }

    private void RefreshUI(int level, int currentXP, int maxXP)
    {
        float ratio = 0f;

        if (maxXP > 0)
            ratio = (float)currentXP / maxXP;

        if (_xpFill != null)
            _xpFill.style.width = Length.Percent(ratio * 100f);

        if (_xpValueLabel != null)
            _xpValueLabel.text = $"{currentXP} / {maxXP}";
    }
}