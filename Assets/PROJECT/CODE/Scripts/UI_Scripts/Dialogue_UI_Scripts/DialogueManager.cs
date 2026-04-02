using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;


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
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Transform responseButtonParent;
    [SerializeField] Button skipButton;
    [SerializeField] Button autoButton;
    [SerializeField] Button maskFakeButton;
    [SerializeField] Image nameBackgroundImage;
    [SerializeField] Image dialogueBackgroundImage;

    [Space(10)]

    [Header("Dictionaries")]
    [SerializedDictionary("Type", "Image"), SerializeField] SerializedDictionary<TypeOfDialogueSpritePosition, Image> dictionaryOfImagesPositions;
    [SerializeField, SerializedDictionary("Character", "Descriptions")] SerializedDictionary<Character, CharacterSO> dictionaryOfDescriptions;
    [SerializeField, SerializedDictionary("Type", "Transform")]
    SerializedDictionary<TypeOfDialogueSpritePosition, Transform> dictionaryOfNamePositions;
    [SerializeField, SerializedDictionary("Type", "Sprite")]
    SerializedDictionary<TypeOfDialogueSpritePosition, Sprite> dictionaryOfNameBackgroundSprites;
    [SerializeField, SerializedDictionary("Type", "Sprite")]
    SerializedDictionary<TypeOfDialogueSpritePosition, Sprite> dictionaryOfDialogueBackgroundSprites;

    [Space(10)]

    [Header("Lists")]
    [SerializeField] List<Sprite> listOfResponsesSprites;
    List<GameObject> _responseButtonsList = new List<GameObject>();


    [Space(10)]

    [Header("Typing Atributes")]
    [SerializeField] float timeBetweenLetters = 0.04f;
    [SerializeField] float timeBetweenEndOfOneLineAndNext = 0.5f;
    [SerializeField] LocalizedString autoOnLocalizeString;
    [SerializeField] LocalizedString autoOffLocalizeString;

    bool _autoPlay;
    bool _dialogueHasEnded;
    bool _onlyOneResponse;
    string _currentFullLine;
    DialogueNode _currentNode;
    PlayerInputHandlerManager _handler;
    Coroutine _typingCoroutine;
    WaitForSeconds _timeBetweenLetterWaitForSeconds;
    WaitForSeconds _timeBetweenEndOfOneLineAndNextWaitForSeconds;



    #region Variables Methods
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetupInitialVariables();

        HideDialogueScreen();

        HideResponseButtons();

        HideNextLineIndicator();

        ResetVariables();

    }
    void SetupInitialVariables()
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
    #endregion

    #region Buttons Methods
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
        if (_autoPlay) autoButton.GetComponentInChildren<TextMeshProUGUI>().text = autoOnLocalizeString.GetLocalizedString();
        else autoButton.GetComponentInChildren<TextMeshProUGUI>().text = autoOffLocalizeString.GetLocalizedString();
    }
    #endregion

    public void InitializeDialogue(DialogueNode node, PlayerInputHandlerManager handler)
    {
        ResetVariables();

        HideNextLineIndicator();

        SetupVariables(node, handler);

        ChangeCharacterSprite(node);

        ChangeDialogueBackground(node);

        ChangeName(node);

        ShowDialogueScreen();

        _typingCoroutine ??= StartCoroutine(DisplayLine());

    }

    void SetupVariables(DialogueNode node, PlayerInputHandlerManager handler)
    {
        _handler = handler;

        _currentNode = node;

        _currentFullLine = node.DialogueText.GetLocalizedString();
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
        if (response.NextNode != null) InitializeDialogue(response.NextNode, _handler);
        else HideDialogueScreen();
    }

    void ChangeCharacterSprite(DialogueNode node)
    {
        // PRIMARY CHARACTER
        TypeOfDialogueSpritePosition spriteType = node.PrimarySpritePosition;

        var dictionaryOfSprites = dictionaryOfDescriptions[node.PrimaryCharacter].DictionaryOfExpressions;

        Sprite newSprite = dictionaryOfSprites[node.PrimaryCharacterExpression];

        dictionaryOfImagesPositions[spriteType].sprite = newSprite;


        // SECONDARY CHARACTER
        if (!node.hasSecondaryCharacterExpression) return;

        TypeOfDialogueSpritePosition secondarySpriteType = node.SecondarySpritePosition;

        var secondaryDictionaryOfSprites = dictionaryOfDescriptions[node.SecondaryCharacter].DictionaryOfExpressions;

        Sprite newSecondarySprite = secondaryDictionaryOfSprites[node.SecondaryCharacterExpression];

        dictionaryOfImagesPositions[secondarySpriteType].sprite = newSecondarySprite;
    }

    void ChangeName(DialogueNode node)
    {
        string currentCharacterName = dictionaryOfDescriptions[node.PrimaryCharacter].CharacterName.GetLocalizedString();

        nameText.text = currentCharacterName;

        nameBackgroundImage.transform.position = dictionaryOfNamePositions[node.PrimarySpritePosition].position;
        nameBackgroundImage.sprite = dictionaryOfNameBackgroundSprites[node.PrimarySpritePosition];
    }

    void ChangeDialogueBackground(DialogueNode node)
    {
        dialogueBackgroundImage.sprite = dictionaryOfDialogueBackgroundSprites[node.PrimarySpritePosition];
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
        foreach (GameObject child in _responseButtonsList)
        {
            child.SetActive(false);
        }

        var responses = _currentNode.Responses;

        for (int i = 0; i < responses.Count; i++)
        {
            var response = responses[i];

            GameObject buttonObj = ReturnResponseButton();

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseText.GetLocalizedString();

            if (responses.Count == 4) buttonObj.GetComponent<Image>().sprite = listOfResponsesSprites[i];
            else buttonObj.GetComponent<Image>().sprite = listOfResponsesSprites[i + 1];

            buttonObj.GetComponent<Button>().onClick.RemoveAllListeners();
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));

            buttonObj.SetActive(true);
        }

        responseButtonParent.gameObject.SetActive(true);
    }

    GameObject ReturnResponseButton()
    {
        for (int i = 0; i < _responseButtonsList.Count; i++)
        {
            if (!_responseButtonsList[i].activeSelf) return _responseButtonsList[i];
        }

        GameObject newButton = Instantiate(responseButtonPrefab, responseButtonParent);
        newButton.SetActive(false);
        _responseButtonsList.Add(newButton);
        return newButton;
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
