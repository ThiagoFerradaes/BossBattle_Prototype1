using UnityEngine;

public class AditionalUIManager : MonoBehaviour
{

    public static AditionalUIManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void InstantiateUI(GameObject ui) {
        GameObject go = Instantiate(ui, this.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
    }
}
