using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public static event Action OnCutsceneStart;
    
    public static event Action OnCutsceneEnd;

    private void EndCutscene()
    {
        eventCutsceneEnd.Invoke();
        OnCutsceneEnd?.Invoke();
        if (playerCanMovement == PlayerMovement.Yes) return;
        
        if(!PlayerManager.Instance.Player.TryGetComponent(out PlayerMovementManager movement))return;
        movement.BlockMovement(false);
    }

    [Header("Events")]
    public UnityEvent eventCutscene;
    public UnityEvent eventCutsceneEnd;
    
    [Space(10)]
    [Header("Types")]
    public TypeCutscene typeCutscene;
    public TypeStartCutscene typeStartCutscene;
    public PlayerMovement playerCanMovement;
    
    [Space(10)]
    [Header("Cutscene")]
    public VideoPlayer videoPlayer;
    public PlayableDirector timelineClip;

    
    public enum TypeCutscene
    {
        PlayableDirector,
        VideoPlayer,
    }

    public enum TypeStartCutscene
    {
        Start,
        Enable,
        Awake,
        Function,
        Events,
        TriggerEnter,
        TriggerExit,
        CollisionEnter,
        CollisionExit,
    }

    public enum PlayerMovement
    {
        No,
        Yes,
    }
    
    private void OnEnable()
    {
        if (typeStartCutscene == TypeStartCutscene.Events)
        {
            eventCutscene.RemoveListener(StartCutsceneOn);
            eventCutscene.AddListener(StartCutsceneOn);
            return;
        }
        
        if(typeStartCutscene != TypeStartCutscene.Enable) return;
        StartCutsceneOn();
    }
    
    private void Start()
    {
        if(typeStartCutscene != TypeStartCutscene.Start) return;
        StartCutsceneOn();
    }
    
    private void Awake()
    {
        if (typeStartCutscene != TypeStartCutscene.Awake)
        {
            return;
        }
        StartCutsceneOn();
    }

    private void OnCollisionEnter()
    {
        if (typeStartCutscene != TypeStartCutscene.CollisionEnter) return;
        StartCutsceneOn();
    }

    private void OnCollisionExit()
    {
        if (typeStartCutscene != TypeStartCutscene.CollisionExit) return;
        StartCutsceneOn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (typeStartCutscene != TypeStartCutscene.TriggerEnter) return;
        StartCutsceneOn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (typeStartCutscene != TypeStartCutscene.TriggerExit) return;
        StartCutsceneOn();
    }

    public void StartCutsceneOn()
    {
        OnCutsceneStart?.Invoke();
        
        switch (typeCutscene)
        {
            case TypeCutscene.PlayableDirector:
                timelineClip.stopped += _ => EndCutscene();
                timelineClip.time = 0;
                timelineClip.Play();
                break;
            case TypeCutscene.VideoPlayer:
                videoPlayer.seekCompleted += _ => EndCutscene();
                videoPlayer.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (playerCanMovement == PlayerMovement.Yes) return;

        if(!PlayerManager.Instance.Player.TryGetComponent(out PlayerMovementManager movement))return;
        movement.BlockMovement(true);
    }
}
