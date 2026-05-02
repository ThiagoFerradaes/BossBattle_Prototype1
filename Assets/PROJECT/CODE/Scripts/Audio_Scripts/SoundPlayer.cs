using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] List<AK.Wwise.Event> listOfSounds;

    public void PlaySound(int soundIndex) => listOfSounds[soundIndex].Post(gameObject);
}
