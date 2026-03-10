using UnityEngine;

public class PlayerProfileManager : MonoBehaviour
{
    public static PlayerProfileManager Instance;

    [Header("커스터마이징 데이터")]
    public PlayerCustomizationData customizationData = new PlayerCustomizationData();

    [Header("상태")]
    public bool isNewGame = true;
    public bool hasFinishedCustomization = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ResetToDefault()
    {
        customizationData = new PlayerCustomizationData();
        isNewGame = true;
        hasFinishedCustomization = false;
    }

    public void SetCustomizationData(PlayerCustomizationData newData)
    {
        customizationData.nickname = newData.nickname;

        customizationData.bodyIndex = newData.bodyIndex;
        customizationData.hairIndex = newData.hairIndex;
        customizationData.beardIndex = newData.beardIndex;
        customizationData.hatIndex = newData.hatIndex;
        customizationData.bagIndex = newData.bagIndex;

        customizationData.hairColorIndex = newData.hairColorIndex;
        customizationData.hatColorIndex = newData.hatColorIndex;
    }
}