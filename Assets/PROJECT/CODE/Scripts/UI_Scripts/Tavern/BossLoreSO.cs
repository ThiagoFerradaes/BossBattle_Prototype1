using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Lore {
    [TextArea(10, 30)] public string LoreText;
}
[CreateAssetMenu(menuName = "Bosses/BossLoreSO")]
public class BossLoreSO : ScriptableObject
{
    public List<Lore> ListOfLoreText = new();
}
