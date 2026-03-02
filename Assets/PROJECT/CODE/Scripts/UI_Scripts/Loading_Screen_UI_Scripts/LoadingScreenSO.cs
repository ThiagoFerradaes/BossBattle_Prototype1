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
    public int SceneIndex;
    public Sprite SavingIcon;
    public List<Sprite> ListOfBackgrounds;
    public List<Tip> ListOfTips;
}
