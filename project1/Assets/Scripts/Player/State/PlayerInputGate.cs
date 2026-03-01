using UnityEngine;

public class PlayerInputGate : MonoBehaviour
{
    public bool IsLocked { get; private set; }

    public void Lock()
    {
        IsLocked = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unlock()
    {
        IsLocked = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Input.anyKeyDown) Debug.Log("AnyKeyDown");
        if (Input.GetKeyDown(KeyCode.W)) Debug.Log("W Down");
    }
}