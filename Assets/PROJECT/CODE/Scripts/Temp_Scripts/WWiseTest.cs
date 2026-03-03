using UnityEngine;

public class WWiseTest : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event myEvent = null;
    void Start()
    {
        myEvent.Post(gameObject);
    }

}
