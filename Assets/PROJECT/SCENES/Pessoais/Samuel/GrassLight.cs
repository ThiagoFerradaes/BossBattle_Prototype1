using UnityEngine;

public class GrassLight : MonoBehaviour
{

    [SerializeField]
    Material material;

    Vector3 lightPosition;
    Vector3 lightBorderPosition;
    Vector3 lightSizeVector;
    float lightSize;

    void Start()
    {
        lightPosition = transform.position;
        Debug.Log("light Position:" + lightPosition);
        lightBorderPosition = transform.GetChild(0).transform.position;
        Debug.Log("light Border Position:" + lightBorderPosition);
        
        lightSizeVector = lightBorderPosition - lightPosition;
        lightSize = lightSizeVector.magnitude;
        Debug.Log("Light Size:" + lightSize);

        material.SetVector("_Light_Obj_Position", lightPosition);
        material.SetFloat("_Light_Size", lightSize);
    }
}
