using UnityEngine;

public class Item : ScriptableObject
{
    public Sprite ItemSprite;
    public string ItemName;
    [TextArea(2,8)]public string ItemDescription;
}
