using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIButtonClickSoundBinder : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UIDocument doc;
    [SerializeField] private AudioManager audioManager;
    
    [Header("UIDocument")]
    [SerializeField] private UIDocument[] extraDocs;

    private void OnEnable()
    {
        if (doc == null) doc = GetComponent<UIDocument>();
        if (audioManager == null)
            audioManager = Object.FindFirstObjectByType<AudioManager>();

        if (doc == null || audioManager == null)
        {
            Debug.LogWarning("[UIButtonClickSoundBinder] UIDocument 또는 AudioManager를 찾지 못했습니다.");
            return;
        }

        BindButtons(doc);
        
        if (extraDocs != null)
            foreach (var extra in extraDocs)
                if (extra != null) BindButtons(extra);
    }

    private void BindButtons(UIDocument target)
    {
        var root = target.rootVisualElement;
        var buttons = root.Query<Button>().ToList();
    
        Debug.Log($"[Binder] {target.name} 에서 버튼 {buttons.Count}개 발견");
    
        foreach (var btn in buttons)
        {
            btn.clicked -= audioManager.PlayUIClick;
            btn.clicked += audioManager.PlayUIClick;
            Debug.Log($"[Binder] 버튼 등록: {btn.name}");
        }
    }
}