using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class Tip {
    public int TipIndex;
    public LocalizedString TipDescription;
}

[CreateAssetMenu(menuName = "LoadingScreenSO")]
public class LoadingScreenSO : ScriptableObject
{
    public int SceneIndex;
    public Sprite SavingIcon;
    public List<Sprite> ListOfBackgrounds;
    public List<Tip> ListOfTips;
}
