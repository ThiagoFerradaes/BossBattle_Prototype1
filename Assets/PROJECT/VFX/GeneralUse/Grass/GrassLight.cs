using UnityEngine;

public class GrassLight : MonoBehaviour
{

    [SerializeField]
    Material material;

    [SerializeField]
    [Tooltip("Chooses whether to update light size and position every frame or at the start of gameplay only")]
    bool updateOnRealTime = false;

    Vector3 lightPosition;
    Vector3 lightBorderPosition;
    Vector3 lightSizeVector;
    float lightSize;

    void Start()
    {
        if (updateOnRealTime == false)
        {
            UpdateLightPosition();
        }
    }
    void Update()
    {
        if (updateOnRealTime == true)
        {
            UpdateLightPosition();
        }
    }

    //pass the global posistion of currect game obj to a material property
    //as well as the distance (lightSize) between current game obj and child game obj (global position)
    private void UpdateLightPosition()
    {
            lightPosition = transform.position;
            lightBorderPosition = transform.GetChild(0).transform.position;

            lightSizeVector = lightBorderPosition - lightPosition;
            lightSize = lightSizeVector.magnitude;

            material.SetVector("_Light_Obj_Position", lightPosition);
            material.SetFloat("_Light_Size", lightSize);
    }
}

