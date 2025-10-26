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
    
    [SerializeField, Tooltip("rectTransform of the image that displays the dialogue text")]
    private RectTransform dialogueBoxRectTransform;
    
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
                dialogueBoxRectTransform.position = dialogueBox.SetPosition(1).position;
                dialogueBoxName.rectTransform.position = dialogueBoxName.positionToChange[0].position;

                art1.backGrand.gameObject.SetActive(true);
                art2.backGrand.gameObject.SetActive(false);
                break;
            case WhosTalkingEnum.Right:
                dialogueBoxName.gameObject.SetActive(true);
                dialogueBoxRectTransform.position = dialogueBox.SetPosition(2).position;
                dialogueBoxName.rectTransform.position = dialogueBoxName.positionToChange[1].position;

                art1.backGrand.gameObject.SetActive(false);
                art2.backGrand.gameObject.SetActive(true);
                break;
            case WhosTalkingEnum.Nothing:
                dialogueBoxName.gameObject.SetActive(false);

                art1.backGrand.gameObject.SetActive(false);
                art2.backGrand.gameObject.SetActive(false);
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
        dialogueBoxName.gameObject.SetActive(false);

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
            // novos tipos fixos de 2 posições
            ImageToChange2Position,
            TextToChange2Position,
            ImageAndBeckGrandToChange2Position,
            TextAndBeckGrandToChange2Position,
            // novos tipos variavel de posições
            ImageAndPositionToChange,
            TextAndPositionToChange,
            ImageAndBeckGrandAndPositionToChange,
            TextAndBeckGrandAndPositionToChange,
        }

        public TypeOfDialogue typeOfDialogue;

        public GameObject gameObject;
        public RectTransform rectTransform;
        public TMP_Text text;
        public Image art;
        public GameObject backGrand;
        public RectTransform[] positionToChange;
        
        
        public DialogueContentUI()
        {
        }

        public DialogueContentUI(TypeOfDialogue typeOfDialogue, GameObject gameObject, RectTransform rectTransform, TMP_Text text, Image art, GameObject backGrand, RectTransform[] positionToChange)
        {
            this.typeOfDialogue = typeOfDialogue;
            this.gameObject = gameObject;
            this.rectTransform = rectTransform;
            this.text = text;
            this.art = art;
            this.backGrand = backGrand;
            this.positionToChange = positionToChange;
        }
    }
    #endregion
    
    #region Editor
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DialogueContentUI))]
    protected class DialogueSystemEditor : PropertyDrawer
    {
        // Helper para altura de uma "linha" padrão
        private float LineH => EditorGUIUtility.singleLineHeight;
        private float LineV => EditorGUIUtility.singleLineHeight + 4f; // pequena folga vertical

        // Guarda o estado aberto/fechado por propriedade (usando o path único)
        private readonly Dictionary<string, bool> _foldouts = new();
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string key = property.propertyPath;
            bool expanded = _foldouts.ContainsKey(key) && _foldouts[key];

            // Altura mínima (só o foldout)
            float total = LineV;

            if (!expanded)
                return total;

            var typeProp = property.FindPropertyRelative("typeOfDialogue");
            var gameObjProp = property.FindPropertyRelative("gameObject");
            var rectProp = property.FindPropertyRelative("rectTransform");
            var textProp = property.FindPropertyRelative("text");
            var artProp = property.FindPropertyRelative("art");
            var backGrandProp = property.FindPropertyRelative("backGrand");
            var positionToChangeProp = property.FindPropertyRelative("positionToChange");

            // campos base (um por linha)
            total += EditorGUI.GetPropertyHeight(typeProp, true) + 2;
            total += EditorGUI.GetPropertyHeight(gameObjProp, true) + 2;
            total += EditorGUI.GetPropertyHeight(rectProp, true) + 2;

            // depende do tipo
            var selectedType = (DialogueContentUI.TypeOfDialogue)typeProp.enumValueIndex;
            switch (selectedType)
            {
                // simples: mostra text OU art
                case DialogueContentUI.TypeOfDialogue.Text:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    break;
                case DialogueContentUI.TypeOfDialogue.Image:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    break;

                // text/art + backGrand
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrand:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    break;
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrand:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    break;

                // listas expandidas (editable)
                case DialogueContentUI.TypeOfDialogue.ImageAndPositionToChange:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(positionToChangeProp, true) + 4;
                    break;
                case DialogueContentUI.TypeOfDialogue.TextAndPositionToChange:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(positionToChangeProp, true) + 4;
                    break;
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandAndPositionToChange:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(positionToChangeProp, true) + 4;
                    break;
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrandAndPositionToChange:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(positionToChangeProp, true) + 4;
                    break;

                // tipos fixos com 2 posições lado-a-lado
                case DialogueContentUI.TypeOfDialogue.ImageToChange2Position:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    total += LineV; // label "Position To Change"
                    total += LineV + 6; // linha dos dois campos + folga
                    break;
                case DialogueContentUI.TypeOfDialogue.TextToChange2Position:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    total += LineV;
                    total += LineV + 6;
                    break;
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandToChange2Position:
                    total += EditorGUI.GetPropertyHeight(artProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    total += LineV;
                    total += LineV + 6;
                    break;
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrandToChange2Position:
                    total += EditorGUI.GetPropertyHeight(textProp, true) + 2;
                    total += EditorGUI.GetPropertyHeight(backGrandProp, true) + 2;
                    total += LineV;
                    total += LineV + 6;
                    break;
            }

            return total+8;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            string key = property.propertyPath;
            
            if (!_foldouts.ContainsKey(key))
                _foldouts[key] = false;
            
            // Foldout principal
            Rect foldoutRect = new(position.x, position.y, position.width, LineH);
            _foldouts[key] = EditorGUI.Foldout(foldoutRect, _foldouts[key], GUIContent.none, true, EditorStyles.foldoutHeader);
            EditorGUI.LabelField(foldoutRect, label, EditorStyles.boldLabel);
            
            if (!_foldouts[key])
            {
                EditorGUI.EndProperty();
                return;
            }
            
            var y = position.y;
            var width = position.width;
            var lh = LineH;
            var lv = LineV;

            // Label do bloco
            EditorGUI.LabelField(new Rect(position.x, y, width, lh), label, EditorStyles.boldLabel);
            y += lv;

            EditorGUI.indentLevel++;

            var typeProp = property.FindPropertyRelative("typeOfDialogue");
            var gameObjProp = property.FindPropertyRelative("gameObject");
            var rectProp = property.FindPropertyRelative("rectTransform");
            var textProp = property.FindPropertyRelative("text");
            var artProp = property.FindPropertyRelative("art");
            var backGrandProp = property.FindPropertyRelative("backGrand");
            var positionToChangeProp = property.FindPropertyRelative("positionToChange");

            // Campos base
            Rect rType = new Rect(position.x, y, width, lh);
            EditorGUI.PropertyField(rType, typeProp);
            y += lv;

            Rect rGame = new Rect(position.x, y, width, lh);
            EditorGUI.PropertyField(rGame, gameObjProp);
            y += lv;

            Rect rRect = new Rect(position.x, y, width, lh);
            EditorGUI.PropertyField(rRect, rectProp);
            y += lv;

            // Switch de desenho
            var selectedType = (DialogueContentUI.TypeOfDialogue)typeProp.enumValueIndex;
            switch (selectedType)
            {
                case DialogueContentUI.TypeOfDialogue.Text:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), textProp);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.Image:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), artProp);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrand:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), artProp);
                    y += lv;
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), backGrandProp);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrand:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), textProp);
                    y += lv;
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), backGrandProp);
                    break;
                }

                // listas expansíveis normais: usamos PropertyField com 'true' para desenhar tudo corretamente
                case DialogueContentUI.TypeOfDialogue.ImageAndPositionToChange:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), artProp);
                    y += lv;
                    float arrH = EditorGUI.GetPropertyHeight(positionToChangeProp, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, width, arrH), positionToChangeProp, true);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.TextAndPositionToChange:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), textProp);
                    y += lv;
                    float arrH = EditorGUI.GetPropertyHeight(positionToChangeProp, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, width, arrH), positionToChangeProp, true);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandAndPositionToChange:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), artProp);
                    y += lv;
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), backGrandProp);
                    y += lv;
                    float arrH = EditorGUI.GetPropertyHeight(positionToChangeProp, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, width, arrH), positionToChangeProp, true);
                    break;
                }
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrandAndPositionToChange:
                {
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), textProp);
                    y += lv;
                    EditorGUI.PropertyField(new Rect(position.x, y, width, lh), backGrandProp);
                    y += lv;
                    float arrH = EditorGUI.GetPropertyHeight(positionToChangeProp, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, width, arrH), positionToChangeProp, true);
                    break;
                }

                // tipos fixos com 2 posições lado a lado
                case DialogueContentUI.TypeOfDialogue.ImageToChange2Position:
                case DialogueContentUI.TypeOfDialogue.TextToChange2Position:
                case DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandToChange2Position:
                case DialogueContentUI.TypeOfDialogue.TextAndBeckGrandToChange2Position:
                {
                    // Desenha principal (art ou text)
                    if (selectedType == DialogueContentUI.TypeOfDialogue.ImageToChange2Position ||
                        selectedType == DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandToChange2Position)
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, width, lh), artProp);
                        y += lv;
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, width, lh), textProp);
                        y += lv;
                    }

                    // Opcional backGrand
                    bool hasBack = selectedType == DialogueContentUI.TypeOfDialogue.ImageAndBeckGrandToChange2Position ||
                                   selectedType == DialogueContentUI.TypeOfDialogue.TextAndBeckGrandToChange2Position;
                    if (hasBack)
                    {
                        EditorGUI.PropertyField(new Rect(position.x, y, width, lh), backGrandProp);
                        y += lv;
                    }

                    // Label
                    EditorGUI.LabelField(new Rect(position.x, y, width, lh),
                        "Position To Change (2 slots)", EditorStyles.boldLabel);
                    y += lv;

                    // Força 2 elementos
                    if (positionToChangeProp.arraySize != 2)
                        positionToChangeProp.arraySize = 2;

                    var labelWidth = 80f;
                    var fieldWidth = (position.width - (labelWidth * 2) - 30f) / 2f;
                    
                    Rect labelRect1 = new(position.x, y, labelWidth, lh);
                    Rect fieldRect1 = new(position.x + labelWidth - 5f, y, fieldWidth, lh);
                    Rect labelRect2 = new(position.x + labelWidth + fieldWidth + 15f, y, labelWidth, lh);
                    Rect fieldRect2 = new(position.x + (labelWidth * 2) + fieldWidth + 10f, y, fieldWidth, lh);

                    EditorGUI.LabelField(labelRect1, "Position 1");
                    EditorGUI.PropertyField(fieldRect1, positionToChangeProp.GetArrayElementAtIndex(0), GUIContent.none);

                    EditorGUI.LabelField(labelRect2, "Position 2");
                    EditorGUI.PropertyField(fieldRect2, positionToChangeProp.GetArrayElementAtIndex(1), GUIContent.none);
                    
                    break;
                }
                default:
                    Debug.LogError("no type");
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }
    
    #endif
    #endregion
}


