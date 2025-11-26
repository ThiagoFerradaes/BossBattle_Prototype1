using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ BossDescription")]
public class BossDescription : ScriptableObject
{
    public Bosses Boss;
    public string BossName, IsleName;
    [TextArea(1,5)]public string Description;
    public Sprite BossSprite;
    public List<LoadingScreenSO> LoadingScreen;
    public List<int> ListOfScenes;
}
