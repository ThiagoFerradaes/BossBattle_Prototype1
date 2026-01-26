using System.Collections.Generic;
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
    
    [HideInInspector] public GameObject player;
    
    private CurrentSelectedCharacterWhiteBoard _playerWhiteBoard;
    
    [SerializeField] private TavernCameraController cameraController;
    [SerializeField] private GameObject cancelTrainingButton;
    [SerializeField] private GameObject canvasSkill;

    private Dictionary<GameObject,GameObject> _playerDictionary = new();

    private Transform _oldPlayer;
    
    private void Awake() 
    {
        _playerWhiteBoard = CurrentSelectedCharacterWhiteBoard.Instance;
        closedInteraction.onClick.AddListener(CloseTrainingField);
        changeCharacterButton.onClick.AddListener(() => characterSelectionManager.Initialize());
    }

    public void SetPlayerInteractionManager(PlayerInteractionManager playerInteractionManager)
    {
        _playerInteractionManager = playerInteractionManager;
        
        foreach (var canvas in canvasActive) canvas.SetActive(true);
        foreach (var canvas in canvasDisabled) canvas.SetActive(false);
        
        //characterSelectionManager.Initialize();
    }

    public void CloseTrainingField()
    {
        if (player is not null)
        {
            if (_oldPlayer is not null)
            {
                cameraController.SetPlayerTransform(_oldPlayer);
                PlayerManager.Instance.SetPlayer(_oldPlayer.gameObject);
                
                player.SetActive(false);
                //Destroy(player);
            }

            cancelTrainingButton.SetActive(false);
            canvasSkill.SetActive(false);
        }
        else
        {
            NoCanvas(); 
        }

        if (_oldPlayer is null)
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
        _oldPlayer = cameraController.GetPlayerTransform();
        cancelTrainingButton.SetActive(true);
        canvasSkill.SetActive(true);
        Character currentCharacter = _playerWhiteBoard.ReturnSelectedCharacter();

        if (characterPrefabDictionary.ContainsKey(currentCharacter)) 
        {
            GameObject instantiate;
            
            if (!_playerDictionary.ContainsKey(characterPrefabDictionary[currentCharacter]))
            {
                instantiate = Instantiate(characterPrefabDictionary[currentCharacter], spawnTransform.position, Quaternion.identity);
                _playerDictionary.Add(characterPrefabDictionary[currentCharacter], instantiate);
            }
            else
            {
                instantiate = _playerDictionary[characterPrefabDictionary[currentCharacter]];
                instantiate.SetActive(true);
            }
            
            player = instantiate;
            PlayerManager.Instance.SetPlayer(player);
            cameraController.SetPlayerTransform(instantiate.transform);
            
        }
        NoCanvas();
    }
    
}
