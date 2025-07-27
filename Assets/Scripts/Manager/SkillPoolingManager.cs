using System.Collections.Generic;
using UnityEngine;

public class SkillPoolingManager : MonoBehaviour {
    #region Parameters
    public static SkillPoolingManager Instance;

    // Dicionários
    Dictionary<string, List<GameObject>> listOfHitboxes = new();
    Dictionary<string, GameObject> listOfManagers = new();

    // Transforms
    public Transform HitboxContainer, ManagerContainer;

    #endregion

    #region Methods
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public GameObject ReturnHitboxFromPool(string objectName, GameObject prefab) {

        if (!listOfHitboxes.ContainsKey(objectName)) {
            listOfHitboxes[objectName] = new List<GameObject>();
        }

        var list = listOfHitboxes[objectName];

        for (int i = 0; i < list.Count; i++) {
            if (!list[i].activeInHierarchy) return list[i];
        }

        GameObject newObject = Instantiate(prefab, HitboxContainer);
        newObject.SetActive(false);
        list.Add(newObject);
        return newObject;
    }

    public GameObject ReturnManagerFromPool(string managerName, GameObject prefab) {

        if (listOfManagers.TryGetValue(managerName, out GameObject manager)) {
            return manager;
        }

        else {
            GameObject newManager = Instantiate(prefab, ManagerContainer);
            newManager.SetActive(false);
            listOfManagers[managerName] = newManager;
            return listOfManagers[managerName];
        }
    }

    #endregion
}
