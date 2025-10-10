using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]private DialogueSystemSo dialogues;
    
    [SerializeField] private UITextButton dialogueBox;
    
    [SerializeField] private List<UITextButton> textBoxesChoice = new();
    
    [SerializeField] private GameObject dialogueBoxChoice;

    [SerializeField] private Image art;
    
    private ConfigurationSo _config;

    private byte _index;
    
    private readonly List<TMP_Text> _textBoxesList = new();
    
    private bool _isChoice;
    private void OnEnable()
    {
        _config = GameConfig.Config;
        
        if (_config == null) return;
        
        _config.OnLanguageChanged += UpdateLanguage;
        
        UpdateLanguage(_config.GetLanguage());
    }
    
    private void OnDisable()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }
    
    private void UpdateLanguage(EnumLanguage lang)
    {
        string newText;

        if (_isChoice)
        {
            for (var index = 0; index < _textBoxesList.Count; index++)
            {
                newText = dialogues.GetChoice[index].GetText.GetText(lang);
                _textBoxesList[index].text = newText;
            }
            return;
        }
        
        newText = dialogues.GetTextBoxesSo(_index).text.GetText(lang);

        if (newText is null)
        {
            Debug.LogError($"erro no pegada de texto {newText} index = {_index} meu dialogues {dialogues} e o dialogo do index e {dialogues.GetTextBoxesSo(_index)} lista de todos e quantidade :{dialogues.GetListTextBoxesSo.Count}");
            return;
        }

        if (_textBoxesList.Count == 0)
        {
            art.sprite = dialogues.GetTextBoxesSo(0).sprite;
            dialogueBox.text.text = newText;
            return;
        }
        
        _textBoxesList[0].text = newText;
        
    }

    public void NextDialogue()
    {
        _textBoxesList.Clear();
        _index++;

        if(dialogues.GetListTextBoxesSo.Count > _index)
        {
            _textBoxesList.Add(dialogueBox.text);
            art.sprite = dialogues.GetTextBoxesSo(_index).sprite;
        }
        else
        {
            if (dialogues.GetChoice.Count is 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _isChoice = true;
            dialogueBoxChoice.SetActive(true);
            dialogueBox.gameObject.SetActive(false);
            foreach (var t in textBoxesChoice)
            {
                _textBoxesList.Add(t.text);
                t.gameObject.SetActive(true);
            }
        }

        UpdateLanguage(_config.GetLanguage());
    }
    
    public void Choice(int value)
    {
        var nextDialogueSystem = dialogues.GetChoice[value].GetNextDialogueSystem;

        if (nextDialogueSystem is null)
        {

            gameObject.SetActive(false);
            return;
        }

        NewDialogue(nextDialogueSystem);
        UpdateLanguage(_config.GetLanguage());
    }
 
    public void NewDialogue(DialogueSystemSo dialogueSystemSo)
    {
        _isChoice = false;
        _index = 0;
        _textBoxesList.Clear();
        dialogues = dialogueSystemSo;
        gameObject.SetActive(true);
        dialogueBox.gameObject.SetActive(true);
        dialogueBoxChoice.SetActive(false);
        UpdateLanguage(_config.GetLanguage());
    }
    
}
