using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool isInteractable = true;

    public virtual void TryInteract()
    {
        if (!isInteractable) return;
        else Interact();
    }

    protected abstract void Interact();
}