using AYellowpaper.SerializedCollections;
using UnityEngine;


public enum ProgressBools { IsKrakenDefeated, HasTalkedToLilianBGFDemo }

public class ProgressWhiteBoard : MonoBehaviour {

    public static ProgressWhiteBoard Instance;

    public SerializedDictionary<ProgressBools, bool> DictionaryOfProgressBools = new();

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
}
