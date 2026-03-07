using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    // 저장 슬롯 개수
    public const int SLOT_COUNT = 3;

    /// 저장 파일 경로 반환
    private static string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    /// 슬롯에 저장
    public static void Save(int slot, SaveData data)
    {
        if (slot < 1 || slot > SLOT_COUNT)
        {
            Debug.LogError("잘못된 슬롯 번호");
            return;
        }

        string path = GetSavePath(slot);

        data.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);

        Debug.Log($"저장 완료: {path}");
    }

    /// 슬롯에서 불러오기
    public static SaveData Load(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("저장 파일 없음");
            return null;
        }

        string json = File.ReadAllText(path);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        return data;
    }

    /// 슬롯에 저장 데이터 있는지 확인
    public static bool HasSave(int slot)
    {
        string path = GetSavePath(slot);
        return File.Exists(path);
    }

    /// 저장 삭제
    public static void Delete(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("저장 삭제");
        }
    }
}