using System;
using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("현재 레벨")]
    public int currentLevel = 1;

    [Header("현재 경험치")]
    public int currentXP = 0;

    [Header("현재 레벨 최대 경험치")]
    public int maxXP;

    [Header("레벨별 필요 경험치")]
    public int[] levelXPTable =
    {
        10, 15, 20, 28, 38, 50, 65, 83, 105
    };

    public event Action<int, int, int> OnXPChanged;

    private void Awake()
    {
        maxXP = GetMaxXP();
    }

    private void Start()
    {
        NotifyXPChanged();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            LevelUp();
        }

        NotifyXPChanged();
    }

    private void LevelUp()
    {
        currentLevel++;
        maxXP = GetMaxXP();
    }

    private int GetMaxXP()
    {
        if (levelXPTable == null || levelXPTable.Length == 0)
            return 1;

        int safeLevel = Mathf.Max(1, currentLevel);
        int index = safeLevel - 1;

        if (index >= levelXPTable.Length)
            index = levelXPTable.Length - 1;

        return levelXPTable[index];
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public int GetCurrentXP()
    {
        return currentXP;
    }

    public void SetLevelData(int level, int xp)
    {
        currentLevel = Mathf.Max(1, level);
        maxXP = GetMaxXP();
        currentXP = Mathf.Clamp(xp, 0, maxXP - 1);

        NotifyXPChanged();
    }

    private void NotifyXPChanged()
    {
        OnXPChanged?.Invoke(currentLevel, currentXP, maxXP);
    }
}