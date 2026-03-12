using UnityEngine;

public class PlayerAppearanceLoader : MonoBehaviour
{
    [SerializeField] private PlayerCustomizer playerCustomizer;

    private void Start()
    {
        ApplyCustomization();
    }

    private void ApplyCustomization()
    {
        // PlayerProfileManager에서 커스텀 데이터 가져오기
        if (PlayerProfileManager.Instance == null)
        {
            Debug.LogWarning("[PlayerAppearanceLoader] PlayerProfileManager.Instance가 null!");
            return;
        }

        if (!PlayerProfileManager.Instance.hasFinishedCustomization)
        {
            Debug.LogWarning("[PlayerAppearanceLoader] 커스터마이징이 완료되지 않았습니다.");
            return;
        }

        PlayerCustomizationData data = PlayerProfileManager.Instance.customizationData;

        if (playerCustomizer == null)
        {
            Debug.LogError("[PlayerAppearanceLoader] PlayerCustomizer가 연결되지 않았습니다!");
            return;
        }

        playerCustomizer.ApplyCustomization(data);
        Debug.Log($"[PlayerAppearanceLoader] 커스터마이징 적용 완료! body:{data.bodyIndex}");
    }
}