using UnityEngine;

public class WWiseTest : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event myEvent = null;
    void Start()
    {
        AkUnitySoundEngine.PostEvent(myEvent.Id, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
