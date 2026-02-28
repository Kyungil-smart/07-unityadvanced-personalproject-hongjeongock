using UnityEngine;

[CreateAssetMenu(menuName = "Game/전리품/리소스 정의")]
public class ResourceDefinition : ScriptableObject
{
    [Header("고유 ID")]
    public string id;   // 예: "wood"

    [Header("표시 이름")]
    public string displayName;

    [Header("아이콘")]
    public Sprite icon;
}