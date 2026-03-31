using AYellowpaper.SerializedCollections;
using System;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfDialogueSprite { Left, Right }
public enum ExpressionTypeDialogue { Angry_S, Angry_N, Anxious_S, Anxious_N, Curious_S, Curious_N,
Default_S, Default_N, Intense_S, Intense_N, Laughing_S, Laughing_N, Satisfied_S, Satisfied_N, Surprise_S, Surprised_N, Vulnerable_S,
Vulnerable_N}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] GameObject dialogueObject;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject responseButtonPrefab;
    [SerializeField] Transform responseButtonParent;
    [SerializedDictionary("Type", "Image")] SerializedDictionary<TypeOfDialogueSprite, Image> dialogueSprites;
    [SerializeField,SerializedDictionary("Character", "ExpressionInfo")] SerializedDictionary<Character, CharacterSO> dictionaryOfExpressions;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogueScreen();
    }

    public void InitializeDialogue(DialogueNode node)
    {
        dialogueText.text = node.DialogueText;

        ShowDialogue();

        foreach (Transform child in responseButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var response in node.Responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseText;

            buttonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectResponse(response));
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

    void ShowDialogue()
    {
        dialogueObject.SetActive(true);
    }
    void HideDialogueScreen()
    {
        dialogueObject.SetActive(false);
    }
}
