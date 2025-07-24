using System.Collections.Generic;
using UnityEngine;

public class SkillPoolingManager : MonoBehaviour
{
    #region Parameters
    public static SkillPoolingManager Instance;

    Dictionary<string, List<GameObject>> listOfObjects = new();
    #endregion

    #region Methods
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public GameObject ReturnObjectFromPool(string objectName, GameObject prefab) {

        var list = listOfObjects[objectName];

        for (int i = 0; i < list.Count; i++) {
            if (!list[i].activeInHierarchy) return list[i];
        }

        GameObject newObject = Instantiate(prefab);
        newObject.SetActive(false);
        list.Add(newObject);
        return newObject;
    }

    #endregion
}
