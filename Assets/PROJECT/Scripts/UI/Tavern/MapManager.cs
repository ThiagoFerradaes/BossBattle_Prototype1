using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    [Foldout("Dictionary"), SerializedDictionary("Fog phase", " Fog object"), SerializeField]
    SerializedDictionary<Phases, GameObject> DictionaryOfFogs = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Phases, GameObject> DictionaryOfButtons = new();
    [Foldout("Dictionary"), SerializedDictionary("Boss phase", " Boss Description"), SerializeField]
    SerializedDictionary<Phases, BossDescription> DictinaryOfDescritions = new();
    [Foldout("Dictionary"), SerializedDictionary("Character phase", " Character Object"), SerializeField]
    SerializedDictionary<CharacterSO, GameObject> DictionaryOfCharacterButtons = new();

    [Foldout("First Map"), SerializeField] Button CloseMapButton;

    [Foldout("Second Map"), SerializeField] GameObject SecondMap;
    [Foldout("Second Map"), SerializeField] Image BossImage;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossName;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossDescription;
    [Foldout("Second Map"), SerializeField] Button CloseSecondMapButton;
    [Foldout("Second Map"), SerializeField] Button SailButton;
    [Foldout("Second Map"), SerializeField] Color characterSelectedColor;
    [Foldout("Second Map"), SerializeField] Color characterDeselectedColor;

    int _nextSceneIndex;

    private void Awake() {
        SetButtons();
    }
    private void OnEnable() {
        WhiteBoard board = WhiteBoard.Instance;

        List<Phases> listOfPhases = board.ReturnListOfUnlockedPhases();
        List<Character> listOfCharacters = board.ReturnListOfUnlockedCharecters();

        foreach (var phase in listOfPhases) { // FOGS
            if (DictionaryOfFogs.ContainsKey(phase)) {
                DictionaryOfFogs[phase].SetActive(false);
            }
        }

        foreach (var phase in DictionaryOfButtons.Keys) { // ILHAS

            Button button = DictionaryOfButtons[phase].GetComponent<Button>();
            button.interactable = listOfPhases.Contains(phase);

        }

        foreach (var cha in DictionaryOfCharacterButtons) { // PERSONAGENS

            GameObject charObj = DictionaryOfCharacterButtons[cha.Key];
            charObj.GetComponentInChildren<Button>().interactable = listOfCharacters.Contains(cha.Key.Character); // BOTAO

            foreach (Transform child in charObj.transform) { // ICONE DE CADEADO
                if (child.CompareTag("LockIcon")) child.gameObject.SetActive(!listOfCharacters.Contains(cha.Key.Character)); 
            }

        }
    }

    void SetButtons() {
        foreach (var pair in DictionaryOfButtons) {  // ILHAS
            if (!DictinaryOfDescritions.TryGetValue(pair.Key, out var description)) continue;

            Button button = pair.Value.GetComponent<Button>();
            var localDescription = description;

            button.onClick.AddListener(() => TurnScreenOn(localDescription));
        }

        foreach (var charButton in DictionaryOfCharacterButtons) {
            Button button = charButton.Value.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => SelectACharacter(charButton.Key));
        }

        CloseMapButton.onClick.AddListener(() => gameObject.SetActive(false));
        CloseSecondMapButton.onClick.AddListener(() => SecondMap.SetActive(false));
        SailButton.onClick.AddListener(Sail);
    }

    void SelectACharacter(CharacterSO character) {
        PlayerWhiteBoard.Instance.SetSelectedCharacter(character);

        foreach (var obj in DictionaryOfCharacterButtons) {
            if (obj.Key.Character == character.Character) {
                obj.Value.transform.GetChild(0).GetComponent<Image>().color = characterSelectedColor;
            }
            else obj.Value.transform.GetChild(0).GetComponent<Image>().color = characterDeselectedColor;
        }


    }

    void TurnScreenOn(BossDescription description) {
        SecondMap.SetActive(true);
        BossImage.sprite = description.BossSprite;
        BossName.text = description.Name;
        BossDescription.text = description.Description;
        _nextSceneIndex = description.SceneIndex;
    }
    void Sail() {
        SceneManager.LoadScene(_nextSceneIndex);
    }
}
