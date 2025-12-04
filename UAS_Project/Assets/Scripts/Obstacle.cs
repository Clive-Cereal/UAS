using UnityEngine;

public class Obstacle : Interactable
{
    protected override void Interact()
    {
        Destroy(gameObject);
    }
}
