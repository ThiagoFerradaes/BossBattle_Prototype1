using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Components")]
    [SerializeField] GameObject dialogueObject;
    [SerializeField] GameObject responseButtonPrefab;
    [SerializeField] GameObject nextLineIndicator;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Transform responseButtonParent;
    [SerializeField] Button skipButton;
    [SerializeField] Button autoButton;
    [SerializeField] Button maskFakeButton;
    [SerializeField] Image nameBackgroundImage;
    [SerializeField] TextMeshProUGUI nameText;

    [Space(10)]

    [Header("Dictionaries")]
    [SerializedDictionary("Type", "Image"), SerializeField] SerializedDictionary<TypeOfDialogueSpritePosition, Image> dictionaryOfImagesPositions;
    [SerializeField, SerializedDictionary("Character", "Descriptions")] SerializedDictionary<Character, CharacterSO> dictionaryOfDescriptions;
    [SerializeField, SerializedDictionary("Type", "Transform")] 
    SerializedDictionary<TypeOfDialogueSpritePosition, Transform> dictionaryOfNamePositions;
    [SerializeField, SerializedDictionary("Type", "Transform")]
    SerializedDictionary<TypeOfDialogueSpritePosition, Sprite> dictionaryOfNameBackgroundSprites;

    [Space(10)]

    [Header("Typing Atributes")]
    [SerializeField] float timeBetweenLetters = 0.04f;
    [SerializeField] float timeBetweenEndOfOneLineAndNext = 0.5f;

    bool _autoPlay;
    bool _dialogueHasEnded;
    bool _onlyOneResponse;
    string _currentFullLine;
    DialogueNode _currentNode;
    PlayerInputHandlerManager _handler;
    Coroutine _typingCoroutine;
    WaitForSeconds _timeBetweenLetterWaitForSeconds;
    WaitForSeconds _timeBetweenEndOfOneLineAndNextWaitForSeconds;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetupVariables();

        HideDialogueScreen();

        HideResponseButtons();

        HideNextLineIndicator();

        ResetVariables();

    }

    void SetupVariables()
    {
        _timeBetweenLetterWaitForSeconds = new(timeBetweenLetters);
        _timeBetweenEndOfOneLineAndNextWaitForSeconds = new(timeBetweenEndOfOneLineAndNext);

        skipButton.onClick.AddListener(SkipButton);
        maskFakeButton.onClick.AddListener(MaskButton);
        autoButton.onClick.AddListener(AutoButton);
    }

    void ResetVariables()
    {
        _currentFullLine = "";
        _currentNode = null;
        _dialogueHasEnded = false;
        _onlyOneResponse = false;
        _handler = null;
        _typingCoroutine = null;
    }
    void SkipButton()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            EndDisplayLine();
        }
        HideDialogueScreen();
    }
    void MaskButton()
    {
        if (_autoPlay) return;

        if (_typingCoroutine == null)
        {
            if (_dialogueHasEnded) HideDialogueScreen();
            if (_onlyOneResponse) InitializeDialogue(_currentNode.Responses[0].NextNode, _handler);
        }
        else SkipLine();
    }
    void AutoButton()
    {
        _autoPlay = !_autoPlay;
        if (_autoPlay) autoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Auto On";
        else autoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Auto Off";
    }
    public void InitializeDialogue(DialogueNode node, PlayerInputHandlerManager handler)
    {
        ResetVariables();

        HideNextLineIndicator();

        _handler = handler;

        _currentNode = node;

        _currentFullLine = node.DialogueText.GetLocalizedString();

        ChangeSprite(node);

        ChangeName(node);

        ShowDialogueScreen();

        _typingCoroutine ??= StartCoroutine(DisplayLine());

    }

    IEnumerator DisplayLine()
    {
        HideResponseButtons();
        dialogueText.text = "";

        foreach (char letter in _currentFullLine.ToCharArray())
        {
            dialogueText.text += letter;
            yield return _timeBetweenLetterWaitForSeconds;
        }

        yield return _timeBetweenEndOfOneLineAndNextWaitForSeconds;

        EndDisplayLine();
    }

    void EndDisplayLine()
    {
        dialogueText.text = _currentFullLine;

        DecideNextStep();

        _typingCoroutine = null;
    }

    void DecideNextStep()
    {
        if (_currentNode.Responses.Count == 0)
        {
            _dialogueHasEnded = true;
            ShowNextLineIndicator();
            // Chegamos ao último diálogo
        }
        else if (_currentNode.Responses.Count == 1)
        {
            if (_autoPlay)
            {
                InitializeDialogue(_currentNode.Responses[0].NextNode, _handler);
            }
            else
            {
                _onlyOneResponse = true;
                ShowNextLineIndicator();
            }
            // Diálogo sem opções, segue para o próximo
        }
        else ShowResponseButtons(); // Diálogo com opções, mostra os botões
    }

    void SelectResponse(DialogueResponse response)
    {
        if(response.NextNode != null) InitializeDialogue(response.NextNode, _handler);
        else HideDialogueScreen();
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

    void ChangeName(DialogueNode node)
    {
        string currentCharacterName = dictionaryOfDescriptions[node.PrimaryCharacter].CharacterName;

        nameText.text = currentCharacterName;

        nameBackgroundImage.transform.position = dictionaryOfNamePositions[node.PrimarySpritePosition].position;
        nameBackgroundImage.sprite = dictionaryOfNameBackgroundSprites[node.PrimarySpritePosition];
    }
    void SkipLine()
    {
        StopCoroutine(_typingCoroutine);
        EndDisplayLine();
    }





    #region Hide and Show
    void ShowDialogueScreen()
    {
        dialogueObject.SetActive(true);
    }
    void HideDialogueScreen()
    {
        if (_handler != null) _handler.SetCanInput(true);
        dialogueObject.SetActive(false);
    }
    void ShowResponseButtons()
    {
        // Fazer um pooling depois
        foreach (Transform child in responseButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var response in _currentNode.Responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseText.GetLocalizedString();

            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
        }

        responseButtonParent.gameObject.SetActive(true);
    }
    void HideResponseButtons()
    {
        responseButtonParent.gameObject.SetActive(false);
    }
    void ShowNextLineIndicator()
    {
        nextLineIndicator.SetActive(true);
    }
    void HideNextLineIndicator()
    {
        nextLineIndicator.SetActive(false);
    }
    #endregion
}
