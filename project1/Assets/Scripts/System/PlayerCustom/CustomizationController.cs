using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 커스터마이징 씬의 UI를 담당하는 컨트롤러
/// 
/// [Inspector 연결 필요 항목]
/// - bodyOptions      : 몸 프리팹 배열
/// - hairOptions      : 헤어 프리팹 배열
/// - outfitOptions    : 의상 프리팹 배열
/// - previewRoot      : 미리보기 캐릭터가 붙을 Transform
/// - playerNameInput  : 이름 입력 필드
/// - hairColorImage   : 헤어 색상 표시용 Image
/// - skinColorImage   : 피부 색상 표시용 Image
/// </summary>
public class CustomizationController : MonoBehaviour
{
    [Header("외형 옵션 프리팹 배열")]
    [SerializeField] private GameObject[] bodyOptions;
    [SerializeField] private GameObject[] hairOptions;
    [SerializeField] private GameObject[] outfitOptions;

    [Header("미리보기")]
    [SerializeField] private Transform previewRoot;

    [Header("UI 요소")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Image hairColorImage;
    [SerializeField] private Image skinColorImage;

    // 현재 선택 인덱스
    private int bodyIndex = 0;
    private int hairIndex = 0;
    private int outfitIndex = 0;
    private Color hairColor = Color.black;
    private Color skinColor = Color.white;

    // 현재 미리보기 인스턴스
    private GameObject currentBodyPreview;
    private GameObject currentHairPreview;
    private GameObject currentOutfitPreview;

    private void Start()
    {
        // 시작 시 기본 외형 표시
        UpdateBodyPreview();
        UpdateHairPreview();
        UpdateOutfitPreview();
    }

    // ──────────────────────────────────────────
    //  버튼 연결용 public 메서드
    // ──────────────────────────────────────────

    /// <summary>몸 다음/이전 (버튼 OnClick에 연결, direction: 1 or -1)</summary>
    public void CycleBody(int direction)
    {
        if (bodyOptions.Length == 0) return;
        bodyIndex = (bodyIndex + direction + bodyOptions.Length) % bodyOptions.Length;
        UpdateBodyPreview();
    }

    /// <summary>헤어 다음/이전</summary>
    public void CycleHair(int direction)
    {
        if (hairOptions.Length == 0) return;
        hairIndex = (hairIndex + direction + hairOptions.Length) % hairOptions.Length;
        UpdateHairPreview();
    }

    /// <summary>의상 다음/이전</summary>
    public void CycleOutfit(int direction)
    {
        if (outfitOptions.Length == 0) return;
        outfitIndex = (outfitIndex + direction + outfitOptions.Length) % outfitOptions.Length;
        UpdateOutfitPreview();
    }

    /// <summary>헤어 색상 변경 (ColorPicker 또는 버튼 OnClick에 연결)</summary>
    public void SetHairColor(Color color)
    {
        hairColor = color;
        if (hairColorImage != null)
            hairColorImage.color = color;
    }

    /// <summary>피부 색상 변경</summary>
    public void SetSkinColor(Color color)
    {
        skinColor = color;
        if (skinColorImage != null)
            skinColorImage.color = color;
    }

    /// <summary>확인 버튼 → GameManager에 데이터 저장 후 인게임으로</summary>
    public void OnConfirm()
    {
        var data = GameManager.Instance.characterData;
        if (data == null)
        {
            Debug.LogError("GameManager에 CharacterData가 연결되지 않았습니다.");
            return;
        }

        data.playerName = string.IsNullOrWhiteSpace(playerNameInput?.text)
            ? "Player"
            : playerNameInput.text;

        data.selectedBodyIndex  = bodyIndex;
        data.selectedHairIndex  = hairIndex;
        data.selectedOutfitIndex = outfitIndex;
        data.hairColor          = hairColor;
        data.skinColor          = skinColor;

        GameManager.Instance.OnCustomizationComplete();
    }

    // ──────────────────────────────────────────
    //  미리보기 갱신 (내부)
    // ──────────────────────────────────────────

    private void UpdateBodyPreview()
    {
        if (currentBodyPreview != null) Destroy(currentBodyPreview);
        if (bodyOptions.Length > 0 && previewRoot != null)
            currentBodyPreview = Instantiate(bodyOptions[bodyIndex], previewRoot);
    }

    private void UpdateHairPreview()
    {
        if (currentHairPreview != null) Destroy(currentHairPreview);
        if (hairOptions.Length > 0 && previewRoot != null)
            currentHairPreview = Instantiate(hairOptions[hairIndex], previewRoot);
    }

    private void UpdateOutfitPreview()
    {
        if (currentOutfitPreview != null) Destroy(currentOutfitPreview);
        if (outfitOptions.Length > 0 && previewRoot != null)
            currentOutfitPreview = Instantiate(outfitOptions[outfitIndex], previewRoot);
    }
}
