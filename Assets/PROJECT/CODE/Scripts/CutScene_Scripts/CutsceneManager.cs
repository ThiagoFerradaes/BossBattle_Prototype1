using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public double time;

    public static event Action OnCutsceneStart;
    
    public static event Action OnCutsceneEnd;

    private void EndCutscene()
    {
        eventCutscene.Invoke();
        OnCutsceneEnd?.Invoke();
    }

    public UnityEvent eventCutscene;
    
    public TypeCutscene typeCutscene;
    
    public enum TypeCutscene
    {
        PlayableDirector,
        VideoPlayer,
    }
    
    public VideoPlayer videoPlayer;
    
    public PlayableDirector timelineClip;
    
    private void Start()
    {
        time = StartCutsceneOn();
    }
    
    public double StartCutsceneOn()
    {
        OnCutsceneStart?.Invoke();

        switch (typeCutscene)
        {
            case TypeCutscene.PlayableDirector:
                timelineClip.stopped += director => EndCutscene();
                timelineClip.time = 0;
                timelineClip.Play();
                return timelineClip.duration;
                break;
            case TypeCutscene.VideoPlayer:
                videoPlayer.seekCompleted += player => EndCutscene();
                videoPlayer.Play();
                return videoPlayer.time;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return 0;
    }
}
