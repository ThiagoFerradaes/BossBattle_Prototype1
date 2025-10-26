using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyEnum;
using PROJECT.Scripts.Enums;
using UnityEditor;

/// <summary>
/// Manages the dialogue system for displaying text, choices, and handling language localization.
/// Controls dialogue flow, user choices, and UI elements related to the dialogue system.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField, Tooltip("Scriptable object containing dialogue data")]
    private DialogueSystemSo dialogues;

    [SerializeField, Tooltip("Main dialogue text box UI element")]
    private UITextButton dialogueBox;

    [SerializeField, Tooltip("Name of the dialogue box UI element")]
    private DialogueContentUI dialogueBoxName;

    [SerializeField, Tooltip("List of choice text boxes for branching dialogue")]
    private List<UITextButton> textBoxesChoice = new();

    [SerializeField, Tooltip("Container for choice dialogue boxes")]
    private GameObject dialogueBoxChoice;

    [SerializeField, Tooltip("Character/scene artwork display")]
    private DialogueContentUI art1, art2;

    [SerializeField, Tooltip("Person Local Transform")]
    private RectTransform personDialogue1Rect, personDialogue2Rect, nameRect1, nameRect2;

    /// <summary>
    /// Event triggered when a dialogue sequence is completed
    /// </summary>
    public event Action<DialogueSystemSo> OnComplicitEvent;

    #endregion

    #region Private Fields

    private ConfigurationSo _config;
    private byte _index;
    private readonly List<TMP_Text> _textBoxesList = new();
    private bool _isChoice;

    #endregion

    #region Unity Lifecycle Methods

    private void OnEnable()
    {
        InitializeConfiguration();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Private Methods

    private void InitializeConfiguration()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
        ;

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        if (dialogues is null) return;

        UpdateLanguage(_config.GetLanguage());
    }

    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    /// <summary>
    /// Updates dialogue text based on the selected language
    /// </summary>
    /// <param name="lang">Language to display text in</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        if (_isChoice)
        {
            UpdateChoiceTexts(lang);
            return;
        }

        UpdateDialogueText(lang);
    }

    /// <summary>
    /// Updates text for all choice options in the selected language
    /// </summary>
    private void UpdateChoiceTexts(EnumLanguage lang)
    {
        for (var index = 0; index < _textBoxesList.Count; index++)
        {
            var newText = dialogues.Choices[index].Text.GetText(lang);
            _textBoxesList[index].text = newText;
        }
    }

    /// <summary>
    /// Updates main dialogue text in selected language
    /// </summary>
    private void UpdateDialogueText(EnumLanguage lang)
    {
        var newText = dialogues.GetDialogueContent(_index).text.GetText(lang);
        dialogueBoxName.text.text = dialogues.GetDialogueContent(_index).name.GetText(lang);

        if (newText == null)
        {
            LogDialogueError(null);
            return;
        }

        if (_textBoxesList.Count == 0)
        {
            InitializeFirstDialogue(newText);
            return;
        }

        _textBoxesList[0].text = newText;
    }

    private void LogDialogueError(string newText)
    {
        Debug.LogError($"Text retrieval error: text={newText}, index={_index}, dialogues={dialogues}, " +
                       $"current dialogue={dialogues.GetDialogueContent(_index)}, total count:{dialogues.DialogueContents.Count}");
    }

    private void InitializeFirstDialogue(string newText)
    {
        var dialogueContent = dialogues.GetDialogueContent(0);

        art1.art.sprite = dialogueContent.spritePerson1;
        art2.art.sprite = dialogueContent.spritePerson2;


        PersonTalk(dialogueContent);

        dialogueBox.buttonText.text = newText;
    }

    private void CompletedDialogue(DialogueSystemSo obj)
    {
        OnComplicitEvent?.Invoke(obj);
        dialogues = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Advances to the next dialogue segment or shows choices if available
    /// </summary>
    public void NextDialogue()
    {
        _textBoxesList.Clear();
        _index++;

        if (dialogues.DialogueContents.Count > _index)
        {
            ContinueDialogue();
        }
        else
        {
            HandleDialogueEnd();
        }

        UpdateLanguage(_config.GetLanguage());
    }

    /// <summary>
    /// Handles selection of a dialogue choice
    /// </summary>
    /// <param name="value">Index of selected choice</param>
    public void Choice(int value)
    {
        var nextDialogueSystem = dialogues.Choices[value].NextDialogue;

        if (nextDialogueSystem == null)
        {
            CompletedDialogue(dialogues);
            gameObject.SetActive(false);
            return;
        }

        NewDialogue(nextDialogueSystem);
    }

    /// <summary>
    /// Starts a new dialogue sequence
    /// </summary>
    /// <param name="dialogueSystemSo">New dialogue system to use</param>
    public Task NewDialogue(DialogueSystemSo dialogueSystemSo)
    {
        if (dialogues is not null && dialogues != dialogueSystemSo)
        {
            CompletedDialogue(dialogues);
        }

        ResetDialogueState();
        dialogues = dialogueSystemSo;
        SetupDialogueUI();
        UpdateLanguage(_config.GetLanguage());

        return Task.CompletedTask;
    }

    #endregion

    #region Helper Methods

    private void PersonTalk(DialogueContent dialogueContent)
    {
        switch (dialogueContent.talking)
        {
            case WhosTalkingEnum.Left:
                dialogueBoxName.gameObject.SetActive(true);
                dialogueBox.buttonText.rectTransform.position = personDialogue1Rect.position;
                dialogueBoxName.rectTransform.position = nameRect1.position;

                art1.gameObject.SetActive(true);
                art2.gameObject.SetActive(false);
                break;
            case WhosTalkingEnum.Right:
                dialogueBoxName.gameObject.SetActive(true);
                dialogueBox.buttonText.rectTransform.position = personDialogue2Rect.position;
                dialogueBoxName.rectTransform.position = nameRect2.position;

                art1.gameObject.SetActive(false);
                art2.gameObject.SetActive(true);
                break;
            case WhosTalkingEnum.Nothing:
                dialogueBoxName.gameObject.SetActive(false);

                art1.gameObject.SetActive(false);
                art2.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ContinueDialogue()
    {
        _textBoxesList.Add(dialogueBox.buttonText);

        var dialogueContent = dialogues.GetDialogueContent(_index);

        art1.art.sprite = dialogueContent.spritePerson1;
        art2.art.sprite = dialogueContent.spritePerson2;

        PersonTalk(dialogueContent);
    }

    private void HandleDialogueEnd()
    {
        if (dialogues.Choices.Count == 0)
        {
            CompletedDialogue(dialogues);
            gameObject.SetActive(false);
            return;
        }

        ShowChoices();
    }

    private void ShowChoices()
    {
        _isChoice = true;
        dialogueBoxChoice.SetActive(true);
        dialogueBox.gameObject.SetActive(false);

        foreach (var t in textBoxesChoice)
        {
            _textBoxesList.Add(t.buttonText);
            t.gameObject.SetActive(true);
        }
    }

    private void ResetDialogueState()
    {
        _isChoice = false;
        _index = 0;
        _textBoxesList.Clear();
    }

    private void SetupDialogueUI()
    {
        gameObject.SetActive(true);
        dialogueBox.gameObject.SetActive(true);
        dialogueBoxChoice.SetActive(false);
    }

    #endregion

    #region Serializable
    
    [Serializable]
    protected class DialogueContentUI
    {
        public enum TypeOfDialogue
        {
            Image,
            Text,
            ImageAndBeckGrand,
            TextAndBeckGrand,
        }

        public TypeOfDialogue typeOfDialogue;

        public GameObject gameObject;
        public RectTransform rectTransform;
        public TMP_Text text;
        public Image art;
        public GameObject backGrand;

        public DialogueContentUI()
        {
        }

        public DialogueContentUI(TypeOfDialogue typeOfDialogue, GameObject gameObject, RectTransform rectTransform,
            TMP_Text text, Image art, GameObject backGrand)
        {
            this.typeOfDialogue = typeOfDialogue;
            this.gameObject = gameObject;
            this.rectTransform = rectTransform;
            this.text = text;
            this.art = art;
            this.backGrand = backGrand;
        }
    }
    #endregion
    
    #region Editor
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DialogueContentUI))]
    protected class DialogueSystemEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Base lines: typeOfDialogue, gameObject, rectTransform
            int lines = 3;

            var typeProp = property.FindPropertyRelative("typeOfDialogue");
            var selectedType = (DialogueSystem.DialogueContentUI.TypeOfDialogue)typeProp.enumValueIndex;

            // Condicionais:
            switch (selectedType)
            {
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.Text:
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.Image:
                    lines += 1; // text OU art
                    break;
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.ImageAndBeckGrand:
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.TextAndBeckGrand:
                    lines += 2; // text/art + backGrand
                    break;
            }

            // Altura final = linhas * altura de linha + label principal
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            return (lines + 1) * lineHeight; // +1 pra label do grupo
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Layout vertical manual
            var y = position.y;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.LabelField(new Rect(position.x, y, position.width, lineHeight), label, EditorStyles.boldLabel);
            y += lineHeight;

            EditorGUI.indentLevel++;

            // Campos
            var typeProp = property.FindPropertyRelative("typeOfDialogue");
            var gameObjProp = property.FindPropertyRelative("gameObject");
            var rectProp = property.FindPropertyRelative("rectTransform");
            var textProp = property.FindPropertyRelative("text");
            var artProp = property.FindPropertyRelative("art");
            var backGrandProp = property.FindPropertyRelative("backGrand");

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
            y += lineHeight;

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), gameObjProp);
            y += lineHeight;

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), rectProp);
            y += lineHeight;

            // Mostra campos condicionalmente
            var selectedType = (DialogueSystem.DialogueContentUI.TypeOfDialogue)typeProp.enumValueIndex;
            switch (selectedType)
            {
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.Text:
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), textProp);
                    y += lineHeight;
                    break;
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.Image:
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), artProp);
                    y += lineHeight;
                    break;
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.ImageAndBeckGrand:
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), artProp);
                    y += lineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), backGrandProp);
                    y += lineHeight;
                    break;
                case DialogueSystem.DialogueContentUI.TypeOfDialogue.TextAndBeckGrand:
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), textProp);
                    y += lineHeight;
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), backGrandProp);
                    y += lineHeight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }
    #endif
    #endregion
}


