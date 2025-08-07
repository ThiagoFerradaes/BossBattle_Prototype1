using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TypeOfScreen { Pause, Victory, Defeat }
public enum TypeOfButton { Menu, Continue, Exit }
public class ScreensInGameUI : MonoBehaviour {

    public static ScreensInGameUI Instance;

    [Foldout("Dictionary"), SerializedDictionary("Type of Screen", "GameObject"), SerializeField]
    SerializedDictionary<TypeOfScreen, GameObject> dictionaryOfScreens = new();
    [Foldout("Dictionary"), SerializedDictionary("Type of Screen", "GameObject"), SerializeField]
    SerializedDictionary<Button, TypeOfButton> dictionaryOfButtons = new();

    [SerializeField] EventChannel OnBossDead;
    private void Awake() {

        if (Instance == null) Instance = this;
        else Destroy(this);

        foreach (var button in dictionaryOfButtons) {
            SetButton(button.Value, button.Key);
        }

        if (OnBossDead != null) OnBossDead.Event += () => TurnScreenOn(TypeOfScreen.Victory);
    }

    private void OnDestroy() {
        if (OnBossDead != null && this.gameObject.activeInHierarchy) OnBossDead.Event -= () => TurnScreenOn(TypeOfScreen.Victory);
    }
    public void TurnScreenOn(TypeOfScreen type) {
        if (!dictionaryOfScreens.ContainsKey(type)) return;

        Time.timeScale = 0;

        dictionaryOfScreens[type].SetActive(true);
    }

    public void TurnScreenOff(TypeOfScreen type) {
        if (!dictionaryOfScreens.ContainsKey(type)) return;

        Time.timeScale = 1;

        dictionaryOfScreens[type].SetActive(false);
    }

    void SetButton(TypeOfButton type, Button button) {
        switch (type) {
            case TypeOfButton.Menu:
                button.onClick.AddListener(() => SceneManager.LoadScene(0));
                break;
            case TypeOfButton.Continue:
                button.onClick.AddListener(() => TurnScreenOff(TypeOfScreen.Pause));
                break;
            case TypeOfButton.Exit:
                button.onClick.AddListener(() => Application.Quit());
                break;
        }
    }
}
