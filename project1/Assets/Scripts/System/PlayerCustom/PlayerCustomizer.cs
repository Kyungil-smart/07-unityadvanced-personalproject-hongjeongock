using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    [Header("바디")]
    [SerializeField] private GameObject[] bodyObjects;

    [Header("파츠 오브젝트")]
    [SerializeField] private GameObject[] hairObjects;
    [SerializeField] private GameObject[] beardObjects;
    [SerializeField] private GameObject[] hatObjects;
    [SerializeField] private GameObject[] bagObjects;

    [Header("색상 적용 Renderer")]
    [SerializeField] private Renderer hairRenderer;
    [SerializeField] private Renderer hatRenderer;

    [Header("색상 배열")]
    [SerializeField] private Color[] hairColors;
    [SerializeField] private Color[] hatColors;

    public void ApplyCustomization(PlayerCustomizationData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[PlayerCustomizer] ApplyCustomization data is null");
            return;
        }

        SetSingleActive(bodyObjects, data.bodyIndex);
        SetSingleActive(hairObjects, data.hairIndex);
        SetSingleActive(beardObjects, data.beardIndex);
        SetSingleActive(hatObjects, data.hatIndex);
        SetSingleActive(bagObjects, data.bagIndex);

        ApplyColor(hairRenderer, hairColors, data.hairColorIndex);
        ApplyColor(hatRenderer, hatColors, data.hatColorIndex);
    }

    private void SetSingleActive(GameObject[] targets, int index)
    {
        if (targets == null || targets.Length == 0) return;

        if (index < 0 || index >= targets.Length)
            index = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
                targets[i].SetActive(i == index);
        }
    }

    private void ApplyColor(Renderer targetRenderer, Color[] colorArray, int index)
    {
        if (targetRenderer == null) return;
        if (colorArray == null || colorArray.Length == 0) return;
        if (index < 0 || index >= colorArray.Length) return;

        var mats = targetRenderer.materials;
        for (int i = 0; i < mats.Length; i++)
            mats[i].color = colorArray[index];
    }
}