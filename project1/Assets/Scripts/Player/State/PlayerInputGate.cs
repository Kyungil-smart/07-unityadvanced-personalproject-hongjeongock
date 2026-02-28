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
}