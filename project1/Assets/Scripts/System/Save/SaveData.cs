using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Vector3 playerPosition;

    public string nickname;
    public float playerHp;
    public float playerMaxHp;
    public int playerLevel;
    public int playerXP;

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