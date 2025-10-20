using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tip {
    public int TipIndex;
    [TextArea(5,10)] public string TipDescription;
}

[CreateAssetMenu(menuName = "LoadingScreenSO")]
public class LoadingScreenSO : ScriptableObject
{
    public List<Sprite> Sprites;
    public Sprite SavingIcon;
    public List<Tip> TipList;
}
