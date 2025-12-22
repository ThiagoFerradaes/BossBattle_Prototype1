using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingFieldSystem : MonoBehaviour
{
    private PlayerInteractionManager _playerInteractionManager;
    
    [SerializeField]private Button changeCharacterButton;
    [SerializeField] private GameObject[] canvasActive;
    [SerializeField] private GameObject[] canvasDisabled;
    [SerializeField] private CharacterSelectionManager characterSelectionManager;
    [SerializeField] private Button closedInteraction;
    
    [SerializedDictionary("Character", "PraFab"), SerializeField]
    SerializedDictionary<Character, GameObject> characterPrefabDictionary = new();

    [SerializeField] private Transform spawnTransform;
    
    [HideInInspector] public GameObject Player;
    
    CurrentSelectedCharacterWhiteBoard _playerWhiteBoard;
    
    [SerializeField] private GameObject cancelTrainingButton;
    [SerializeField] private GameObject canvasSkill;
    private void Awake() 
    {
        _playerWhiteBoard = CurrentSelectedCharacterWhiteBoard.Instance;
        closedInteraction.onClick.AddListener(CloseTrainingField);
    }

    public void SetPlayerInteractionManager(PlayerInteractionManager playerInteractionManager)
    {
        _playerInteractionManager = playerInteractionManager;
        
        foreach (var canvas in canvasActive) canvas.SetActive(true);
        foreach (var canvas in canvasDisabled) canvas.SetActive(false);
        
        characterSelectionManager.Initialize();
    }

    public void CloseTrainingField()
    {
        if (Player is not null)
        {
            Destroy(Player);    
            cancelTrainingButton.SetActive(false);
            canvasSkill.SetActive(false);
        }
        else
        {
            NoCanvas(); 
        }
        
        _playerInteractionManager.EndInteraction();
        _playerInteractionManager = null;
    }
    
    private void NoCanvas()
    {
        foreach (var canvas in canvasActive) canvas.SetActive(false);
        foreach (var canvas in canvasDisabled) canvas.SetActive(true);
    }
    
    public void Spawn()
    {
        cancelTrainingButton.SetActive(true);
        canvasSkill.SetActive(true);
        Character currentCharacter = _playerWhiteBoard.ReturnSelectedCharacter();

        if (characterPrefabDictionary.ContainsKey(currentCharacter)) {
            GameObject player = Instantiate(characterPrefabDictionary[currentCharacter], spawnTransform.position, Quaternion.identity);
            Player = player;
        }
        NoCanvas();
    }
    
}
