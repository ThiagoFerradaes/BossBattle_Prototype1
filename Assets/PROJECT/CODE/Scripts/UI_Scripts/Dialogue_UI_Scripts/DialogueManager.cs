using AYellowpaper.SerializedCollections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfDialogueSpritePosition { Left, Right }
public enum ExpressionTypeDialogue
{
    Angry_S, Angry_N, Anxious_S, Anxious_N, Curious_S, Curious_N,
    Default_S, Default_N, Intense_S, Intense_N, Laughing_S, Laughing_N, Satisfied_S, Satisfied_N, Surprise_S, Surprised_N, Vulnerable_S,
    Vulnerable_N
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] GameObject dialogueObject;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject responseButtonPrefab;
    [SerializeField] Transform responseButtonParent;
    [SerializedDictionary("Type", "Image"), SerializeField] SerializedDictionary<TypeOfDialogueSpritePosition, Image> dictionaryOfImagesPositions;
    [SerializeField, SerializedDictionary("Character", "Descriptions")] SerializedDictionary<Character, CharacterSO> dictionaryOfDescriptions;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogueScreen();

    }

    public void InitializeDialogue(DialogueNode node, Dialogue rootDialogue = null)
    {
        dialogueText.text = node.DialogueText.GetLocalizedString();

        ChangeSprite(node);

        ShowDialogue();

        foreach (Transform child in responseButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var response in node.Responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseText.GetLocalizedString();

            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
        }
    }

    void SelectResponse(DialogueResponse response)
    {
        if (!response.NextNode.IsLastNode())
        {
            InitializeDialogue(response.NextNode);
        }
        else
        {
            HideDialogueScreen();
        }
    }


    void ChangeSprite(DialogueNode node)
    {
        TypeOfDialogueSpritePosition spriteType = node.PrimarySpritePosition;

        var dictionaryOfSprites = dictionaryOfDescriptions[node.PrimaryCharacter].DictionaryOfExpressions;

        Sprite newSprite = dictionaryOfSprites[node.PrimaryCharacterExpression];

        dictionaryOfImagesPositions[spriteType].sprite = newSprite;

        if (!node.hasSecondaryCharacterExpression) return;

        TypeOfDialogueSpritePosition secondarySpriteType = node.SecondarySpritePosition;

        var secondaryDictionaryOfSprites = dictionaryOfDescriptions[node.SecondaryCharacter].DictionaryOfExpressions;

        Sprite newSecondarySprite = secondaryDictionaryOfSprites[node.SecondaryCharacterExpression];

        dictionaryOfImagesPositions[secondarySpriteType].sprite = newSecondarySprite;
    }


    void ShowDialogue()
    {
        dialogueObject.SetActive(true);
    }
    void HideDialogueScreen()
    {
        dialogueObject.SetActive(false);
    }
}
