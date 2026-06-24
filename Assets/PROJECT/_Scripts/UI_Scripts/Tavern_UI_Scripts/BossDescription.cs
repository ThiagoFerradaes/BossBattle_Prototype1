using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Bosses/ BossDescription")]
public class BossDescription : ScriptableObject
{
    public Bosses Boss;
    public LocalizedSprite IsleName;
    public LocalizedString Description;
    public Sprite BossSprite;
    public List<LoadingScreenSO> LoadingScreen;
    public List<int> ListOfScenes;
}
