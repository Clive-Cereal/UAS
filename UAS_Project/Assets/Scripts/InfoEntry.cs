using UnityEngine;

[CreateAssetMenu(menuName = "Info/Info Entry", fileName = "NewInfoEntry")]
public class InfoEntry : ScriptableObject
{
    public string title;
    [TextArea(4,10)]
    public string description;
}
