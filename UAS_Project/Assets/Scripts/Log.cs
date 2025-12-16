using UnityEngine;

public class Log : Interactable
{
    [SerializeField] private InfoEntry info;

    protected override void Interact()
    {
        if (info == null)
        {
            Debug.LogWarning($"No InfoEntry assigned on {name}");
            return;
        }

        UIManager.Instance?.ShowInfo(info);
    }
}
