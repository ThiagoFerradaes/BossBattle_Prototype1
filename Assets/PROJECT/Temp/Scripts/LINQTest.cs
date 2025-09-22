using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LINQTest : MonoBehaviour
{
    public UniversalRendererData rendererData;
    
    void Start()
    {
        //List<int> listOfNumber = new() { 1, 2, 3, 4 };

        //listOfNumber.Where(n => n > 2).ToList();

        //Debug.Log(string.Join(", ", listOfNumber));


        if (rendererData == null) {
            Debug.LogWarning("Renderer Data não atribuído!");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures) {

            if (feature.name == "FullScreenTakingDamegeShader") {
                feature.SetActive(true);
            }
        }
    }

    private void OnDestroy() {
        foreach (var feature in rendererData.rendererFeatures) {

            if (feature.name == "FullScreenTakingDamegeShader") {
                feature.SetActive(false);
            }
        }
    }
}


