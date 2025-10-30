using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ BossDescription")]
public class BossDescription : ScriptableObject
{
    public int SceneIndex;
    public string Name;
    public LoadingScreenSO LoadingScreen;
    public Sprite BossSprite;
    public Sprite DifficultyIcon;
    [TextArea(1,5)]public string Description;
}
