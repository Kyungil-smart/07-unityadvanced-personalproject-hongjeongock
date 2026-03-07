using System;

[Serializable]
public class SaveData
{
    public string nickname;
    public float playerHp;

    public float posX;
    public float posY;
    public float posZ;

    public int gold;
    public int houseLevel;

    // 자원
    public int wood;
    public int stone;
    public int iron;
    public int coin;

    public string saveTime;
}