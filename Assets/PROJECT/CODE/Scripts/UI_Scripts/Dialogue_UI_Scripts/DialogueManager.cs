using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] GameObject dialogueObject;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject responseButtonPrefab;
    [SerializeField] Transform responseButtonParent;


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
