using UnityEngine;
using UnityEditor;

public class AttachHatsToHead
{
    [MenuItem("Tools/Attach Hats To Head")]
    static void Attach()
    {
        GameObject head = GameObject.Find("Head");

        if (head == null)
        {
            Debug.LogError("Head bone not found");
            return;
        }

        GameObject[] hats = Selection.gameObjects;

        foreach (GameObject hat in hats)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(hat) as GameObject;

            instance.transform.SetParent(head.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }

        Debug.Log("Hats attached to head");
    }
}