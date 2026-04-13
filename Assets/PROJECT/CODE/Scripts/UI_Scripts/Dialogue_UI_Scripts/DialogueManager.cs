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
    Vulnerable_N, Special_1, Special_2, Special_3
}

public class DialogueManager : MonoBehaviour
{

    #region Variables

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

    #endregion

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

    /// <summary>
    /// Definindo as variáveis de tempo como WaitForSeconds para otimizar o código, e adicionando os listeners dos botões.
    /// </summary>
    void SetupInitialVariables()
    {
        _timeBetweenLetterWaitForSeconds = new(timeBetweenLetters);
        _timeBetweenEndOfOneLineAndNextWaitForSeconds = new(timeBetweenEndOfOneLineAndNext);

        skipButton.onClick.AddListener(SkipButton);
        maskFakeButton.onClick.AddListener(MaskButton);
        autoButton.onClick.AddListener(AutoButton);
    }

    /// <summary>
    /// Resetando váriaveis para o estado inicial para evitar bugs ao iniciar um novo diálogo.
    /// </summary>
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

    /// <summary>
    /// Adicionando funcionalinade no botão de pular
    /// </summary>
    void SkipButton()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            EndDisplayLine();
        }
        HideDialogueScreen();
    }

    /// <summary>
    /// Adicionando funcionalidade no botão que avança o diálogo
    /// </summary>
    void MaskButton()
    {
        if (_dialogueHasEnded)
        {
            HideDialogueScreen();
            return;
        }

        if (_autoPlay) return;

        if (_typingCoroutine == null)
        {
            if (_dialogueHasEnded) HideDialogueScreen();
            if (_onlyOneResponse) InitializeDialogue(_currentNode.Responses[0].NextNode, _handler);
        }
        else SkipLine();
    }

    /// <summary>
    /// Adicionando funcionalidade no botão de auto play
    /// </summary>
    void AutoButton()
    {
        _autoPlay = !_autoPlay;

        if (!_autoPlay) autoButton.GetComponentInChildren<TextMeshProUGUI>().text = autoOffLocalizeString.GetLocalizedString();
        else
        {
            autoButton.GetComponentInChildren<TextMeshProUGUI>().text = autoOnLocalizeString.GetLocalizedString();
            if (_typingCoroutine == null)
            {
                if (_dialogueHasEnded) HideDialogueScreen();
                else if (_onlyOneResponse) InitializeDialogue(_currentNode.Responses[0].NextNode, _handler);
            }
        }
    }

    /// <summary>
    /// Função que pula a animação de digitação e mostra a linha completa
    /// </summary>
    void SkipLine()
    {
        StopCoroutine(_typingCoroutine);
        EndDisplayLine();
    }
    #endregion

    #region Execute Dialogue

    /// <summary>
    /// Função chamada para começar uma linha de diálogo
    /// </summary>
    /// <param name="node"></param>
    /// <param name="handler"></param>
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

    /// <summary>
    /// Função usada para definir variáveis necessárias para o funcionamento do diálogo
    /// </summary>
    /// <param name="node"></param>
    /// <param name="handler"></param>
    void SetupVariables(DialogueNode node, PlayerInputHandlerManager handler)
    {
        _handler = handler;

        _currentNode = node;

        _currentFullLine = node.DialogueText.GetLocalizedString();
    }

    /// <summary>
    /// Corrotina de animação de digitação
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Função chamada no final da animação de digitação, ou quando o jogador decide pular a animação
    /// </summary>
    void EndDisplayLine()
    {
        dialogueText.text = _currentFullLine;

        DecideNextStep();

        _typingCoroutine = null;
    }

    /// <summary>
    /// Função que decide o que acontece quando uma linha de diálogo termina
    /// </summary>
    void DecideNextStep()
    {
        if (_currentNode.Responses.Count == 0 || _currentNode.Responses == null)
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

    #endregion

    #region Change Dialogue UI
    /// <summary>
    /// Função que muda o sprite do personagem de acordo com o diálogo
    /// </summary>
    /// <param name="node"></param>
    void ChangeCharacterSprite(DialogueNode node)
    {
        // PRIMARY CHARACTER
        TypeOfDialogueSpritePosition spriteType = node.PrimaryCharacter == Character.Julian? TypeOfDialogueSpritePosition.Left:
            TypeOfDialogueSpritePosition.Right;

        var dictionaryOfSprites = dictionaryOfDescriptions[node.PrimaryCharacter].DictionaryOfExpressions;

        Sprite newSprite = dictionaryOfSprites[node.PrimaryCharacterExpression];

        dictionaryOfImagesPositions[spriteType].sprite = newSprite;


        // SECONDARY CHARACTER
        if (!node.hasSecondaryCharacterExpression) return;

        TypeOfDialogueSpritePosition secondarySpriteType = node.SecondaryCharacter == Character.Julian ? TypeOfDialogueSpritePosition.Left :
            TypeOfDialogueSpritePosition.Right;

        var secondaryDictionaryOfSprites = dictionaryOfDescriptions[node.SecondaryCharacter].DictionaryOfExpressions;

        Sprite newSecondarySprite = secondaryDictionaryOfSprites[node.SecondaryCharacterExpression];

        dictionaryOfImagesPositions[secondarySpriteType].sprite = newSecondarySprite;
    }

    /// <summary>
    /// Função que muda a posição da imagem de fundo do nome e o nome mostrado de acordo com o diálogo
    /// </summary>
    /// <param name="node"></param>
    void ChangeName(DialogueNode node)
    {
        string currentCharacterName = dictionaryOfDescriptions[node.PrimaryCharacter].CharacterName.GetLocalizedString();

        nameText.text = currentCharacterName;

        TypeOfDialogueSpritePosition position = node.PrimaryCharacter == Character.Julian? TypeOfDialogueSpritePosition.Left:
            TypeOfDialogueSpritePosition.Right;
        nameBackgroundImage.transform.position = dictionaryOfNamePositions[position].position;
        nameBackgroundImage.sprite = dictionaryOfNameBackgroundSprites[position];
    }

    /// <summary>
    /// Função que muda o sprite de fundo do diálogo de acordo com o diálogo
    /// </summary>
    /// <param name="node"></param>
    void ChangeDialogueBackground(DialogueNode node)
    {
        TypeOfDialogueSpritePosition position = node.PrimaryCharacter == Character.Julian ? TypeOfDialogueSpritePosition.Left :
    TypeOfDialogueSpritePosition.Right;
        dialogueBackgroundImage.sprite = dictionaryOfDialogueBackgroundSprites[position];
    }

    #endregion

    #region Hide and Show
    /// <summary>
    /// Função para ligar a UI de diálogo
    /// </summary>
    void ShowDialogueScreen()
    {
        dialogueObject.SetActive(true);
    }

    /// <summary>
    /// Função para desligar a UI de diálogo e permitir que o jogador volte a controlar o personagem
    /// </summary>
    void HideDialogueScreen()
    {
        if (_handler != null) _handler.SetCanInput(true);
        dialogueObject.SetActive(false);
    }

    /// <summary>
    /// Função que mostra as opções de resposta do diálogo
    /// </summary>
    void ShowResponseButtons()
    {

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

    /// <summary>
    /// Função que retorna os botões de resposta a partir de um pooling
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Função atribuida aos botões de resposta
    /// </summary>
    /// <param name="response"></param>
    void SelectResponse(DialogueResponse response)
    {
        if (response.NextNode != null) InitializeDialogue(response.NextNode, _handler);
        else HideDialogueScreen();
    }

    /// <summary>
    /// Função que esconde os botões de resposta
    /// </summary>
    void HideResponseButtons()
    {
        responseButtonParent.gameObject.SetActive(false);

        foreach (GameObject child in _responseButtonsList)
        {
            child.SetActive(false);
        }
    }

    /// <summary>
    /// Função que mostra o indicador de próxima linha
    /// </summary>
    void ShowNextLineIndicator()
    {
        nextLineIndicator.SetActive(true);
    }

    /// <summary>
    /// Função que esconde o indicador de próxima linha
    /// </summary>
    void HideNextLineIndicator()
    {
        nextLineIndicator.SetActive(false);
    }
    #endregion
}
