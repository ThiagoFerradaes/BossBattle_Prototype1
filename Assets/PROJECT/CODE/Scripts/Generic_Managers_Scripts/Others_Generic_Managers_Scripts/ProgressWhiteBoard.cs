using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;


public enum ProgressBools { IsKrakenDefeated, HasTalkedToLilianBGFDemo, LilianTwo, TalkedToBastian, TalkedToCyrus, AskedAboutLilian, TalkedtoLilianORBastian, 
hasTalkedToBertrand}

public class ProgressWhiteBoard : MonoBehaviour {

    public static ProgressWhiteBoard Instance;

    public SerializedDictionary<ProgressBools, bool> DictionaryOfProgressBools = new();

    public bool HasSeenDemoPopUp;
    public bool HasSeenPostKrakenPopUp;
    public bool HasSeenKrakenPopUp;

    public event Action<ProgressBools, bool> OnChangedBoolValue;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);

        PopulateDictionary();
    }

    void PopulateDictionary() {
        foreach( var value in System.Enum.GetValues(typeof(ProgressBools))) {
            DictionaryOfProgressBools[(ProgressBools)value] = false;
        }
    }

    public void SetProgressBool(ProgressBools type, bool newValue) {
        DictionaryOfProgressBools[type] = newValue;
        OnChangedBoolValue?.Invoke(type, newValue);
    }
}
