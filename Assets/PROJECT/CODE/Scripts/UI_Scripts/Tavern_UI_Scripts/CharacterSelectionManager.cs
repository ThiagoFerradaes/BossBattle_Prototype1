using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] Image SelectedCharacterImage;
    [SerializeField] Image SelectedCharacterSignature;
    [SerializeField] GameObject CharacterSelectionScreen;
    [SerializedDictionary("Character", "Button"), SerializeField]
    SerializedDictionary<CharacterSO, Button> dictionaryOfCharactersButton = new();

    CharacterSO _currentSelectedCharacterInfo;

    public void Initialize() {
        _currentSelectedCharacterInfo = PlayerWhiteBoard.Instance.ReturnSelectedCharacterSO();

        ChangeSelectedImageAndSignature();
    }

    void ChangeSelectedImageAndSignature() {
        SelectedCharacterImage.sprite = _currentSelectedCharacterInfo.CharacterSelectionImage;
        SelectedCharacterSignature.sprite = _currentSelectedCharacterInfo.CharacterSignature;
    }

    void ChangeSkillsIcon() {

    }
}
