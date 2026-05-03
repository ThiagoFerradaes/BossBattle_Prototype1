using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] List<AK.Wwise.Event> listOfSounds;
    [SerializeField] List<AK.Wwise.Switch> listOfSwitchs;

    public void PlaySound(int soundIndex) => listOfSounds[soundIndex].Post(gameObject);

    public void SetSwitch(int switchIndex) => listOfSwitchs[switchIndex].SetValue(gameObject);
}
