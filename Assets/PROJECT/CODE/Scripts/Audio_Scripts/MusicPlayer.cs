using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    [SerializeField] bool hasSwitch;
    [SerializeField] bool hasAmbience;
    [SerializeField, ShowIf("hasSwitch")] AK.Wwise.Switch musicSwitch;

    [SerializeField] AK.Wwise.Event newMusic = null;
    [SerializeField, ShowIf("hasAmbience")] List<GameObject> ambiencePrefabs = null;

    private void Start() {
        AkUnitySoundEngine.StopAll();

        PlayMusic();
        SpawnAmbiencePrefabs();
    }

    void PlayMusic() {
        if (hasSwitch) musicSwitch.SetValue(gameObject);

        newMusic.Post(gameObject);
    }

    void SpawnAmbiencePrefabs()
    {
        if (!hasAmbience || ambiencePrefabs == null) return;

        foreach (var prefab in ambiencePrefabs)
        {
            if (prefab != null)
            {
                Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
            }
        }
    }
}
