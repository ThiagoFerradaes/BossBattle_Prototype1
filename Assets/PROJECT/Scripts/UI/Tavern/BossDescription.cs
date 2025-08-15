using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/BossDescription")]
public class BossDescription : ScriptableObject
{
    public int SceneIndex;
    public string Name;
    public LoadingScreenSO LoadingScreen;
    public Sprite BossSprite;
    [TextArea(1,5)]public string Description;
}
