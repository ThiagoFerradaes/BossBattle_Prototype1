using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/BossDescription")]
public class BossDescription : ScriptableObject
{
    public int SceneIndex;
    public Sprite BossSprite;
    public string Name;
    [TextArea(1,5)]public string Description;
}
