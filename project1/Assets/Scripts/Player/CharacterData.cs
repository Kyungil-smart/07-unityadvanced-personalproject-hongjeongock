using UnityEngine;

/// <summary>
/// 커스터마이징 선택 결과를 저장하는 데이터 컨테이너 (씬 간 전달용)
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("기본 정보")]
    public string playerName = "Player";

    [Header("외형 선택")]
    public int selectedBodyIndex = 0;
    public int selectedHairIndex = 0;
    public int selectedOutfitIndex = 0;
    public Color hairColor = Color.black;
    public Color skinColor = Color.white;

    /// <summary>
    /// 데이터 초기화 (새 게임 시작 시 호출)
    /// </summary>
    public void Reset()
    {
        playerName = "Player";
        selectedBodyIndex = 0;
        selectedHairIndex = 0;
        selectedOutfitIndex = 0;
        hairColor = Color.black;
        skinColor = Color.white;
    }
}
