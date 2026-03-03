using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    void Interact(GameObject interactor);
    void LockOn(bool isLockOn);
}